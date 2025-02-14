// SPDX-License-Identifier: Apache-2.0
pragma solidity ^0.8.19;

import {IERC1155} from "@0xsequence/erc-1155/contracts/interfaces/IERC1155.sol";
import {IERC1155TokenReceiver} from "@0xsequence/erc-1155/contracts/interfaces/IERC1155TokenReceiver.sol";
import {IERC1155ItemsFunctions} from "@0xsequence/contracts-library/tokens/ERC1155/presets/items/IERC1155Items.sol";
import {Ownable} from "@openzeppelin/contracts/access/Ownable.sol";

// Thrown when token id is invalid
error InvalidTokenId();

// Thrown when mint requirements are not met
error MintRequirementsNotMet();

// Thrown when input array length is invalid
error InvalidArrayLength();

// Thrown when method called is invalid
error InvalidMethod();

interface IERC1155Items is IERC1155ItemsFunctions, IERC1155 {
    function batchBurn(uint256[] memory tokenIds, uint256[] memory amounts) external;
}

struct TokenRequirements {
    uint256 tokenId;
    uint256 amount;
}

contract ERC1155BurnToMint is IERC1155TokenReceiver, Ownable {
    IERC1155Items private immutable ITEMS;

    mapping(uint256 => TokenRequirements[]) public burnRequirements;
    mapping(uint256 => TokenRequirements[]) public holdRequirements;

    constructor(address items, address owner_) {
        Ownable.transferOwnership(owner_);
        ITEMS = IERC1155Items(items);
    }

    /**
     * Contract owner can mint anything
     */
    function mintOpen(address to, uint256 tokenId, uint256 amount) external onlyOwner {
        ITEMS.mint(to, tokenId, amount, "");
    }

    /**
     * Owner sets minting requirements for a token.
     * @dev This function does not validate inputs ids of the inputs.
     * @dev `burnTokenIds` and `holdTokenIds` should not overlap, should be unique and should not contain `mintTokenId`.
     */
    function setMintRequirements(
        uint256 mintTokenId,
        uint256[] calldata burnTokenIds,
        uint256[] calldata burnAmounts,
        uint256[] calldata holdTokenIds,
        uint256[] calldata holdAmounts
    )
        external
        onlyOwner
    {
        if (burnTokenIds.length != burnAmounts.length || holdTokenIds.length != holdAmounts.length) {
            revert InvalidArrayLength();
        }

        delete burnRequirements[mintTokenId];
        delete holdRequirements[mintTokenId];
        for (uint256 i = 0; i < burnTokenIds.length; i++) {
            burnRequirements[mintTokenId].push(TokenRequirements(burnTokenIds[i], burnAmounts[i]));
        }
        for (uint256 i = 0; i < holdTokenIds.length; i++) {
            holdRequirements[mintTokenId].push(TokenRequirements(holdTokenIds[i], holdAmounts[i]));
        }
    }

    /**
     * @notice Use `onERC1155BatchReceived` instead.
     */
    function onERC1155Received(address, address, uint256, uint256, bytes calldata)
        external
        pure
        override
        returns (bytes4)
    {
        revert InvalidMethod();
    }

    /**
     * Receive tokens for burning and mint new token.
     * @dev `data` is abi.encode(mintTokenId).
     */
    function onERC1155BatchReceived(
        address,
        address from,
        uint256[] calldata tokenIds,
        uint256[] calldata amounts,
        bytes calldata data
    )
        external
        override
        returns (bytes4)
    {
        if (msg.sender != address(ITEMS)) {
            // Got tokens from incorrect contract
            revert MintRequirementsNotMet();
        }

        // Check mint requirements
        uint256 mintTokenId = abi.decode(data, (uint256));
        _checkMintRequirements(from, mintTokenId, tokenIds, amounts);

        // Burn these tokens and mint the new token
        ITEMS.batchBurn(tokenIds, amounts);
        ITEMS.mint(from, mintTokenId, 1, "");

        return this.onERC1155BatchReceived.selector;
    }

    /**
     * Checks mint requirements for a token.
     * @dev This function assumes the `burnTokenIds` and `burnAmounts` have been burned.
     */
    function _checkMintRequirements(
        address holder,
        uint256 mintTokenId,
        uint256[] calldata burnTokenIds,
        uint256[] calldata burnAmounts
    )
        internal
        view
    {
        if (burnTokenIds.length != burnAmounts.length || burnTokenIds.length == 0) {
            revert InvalidArrayLength();
        }

        // Check burn tokens sent is correct
        TokenRequirements[] memory requirements = burnRequirements[mintTokenId];
        if (requirements.length != burnTokenIds.length) {
            revert MintRequirementsNotMet();
        }
        for (uint256 i = 0; i < requirements.length; i++) {
            if (requirements[i].tokenId != burnTokenIds[i] || requirements[i].amount != burnAmounts[i]) {
                // Invalid burn token id or amount
                revert MintRequirementsNotMet();
            }
        }

        // Check held tokens
        requirements = holdRequirements[mintTokenId];
        if (requirements.length != 0) {
            address[] memory holders = new address[](requirements.length);
            uint256[] memory holdTokenIds = new uint256[](requirements.length);
            for (uint256 i = 0; i < requirements.length; i++) {
                holders[i] = holder;
                holdTokenIds[i] = requirements[i].tokenId;
            }
            uint256[] memory balances = ITEMS.balanceOfBatch(holders, holdTokenIds);
            for (uint256 i = 0; i < requirements.length; i++) {
                if (balances[i] < requirements[i].amount) {
                    // Not enough held tokens
                    revert MintRequirementsNotMet();
                }
            }
        }
    }

    function getMintRequirements(uint256 mintTokenId)
        external
        view
        returns (
            uint256[] memory burnIds,
            uint256[] memory burnAmounts,
            uint256[] memory holdIds,
            uint256[] memory holdAmounts
        )
    {
        TokenRequirements[] memory requirements = burnRequirements[mintTokenId];
        uint256 requirementsLength = requirements.length;
        burnIds = new uint256[](requirementsLength);
        burnAmounts = new uint256[](requirementsLength);
        for (uint256 i = 0; i < requirementsLength; i++) {
            burnIds[i] = requirements[i].tokenId;
            burnAmounts[i] = requirements[i].amount;
        }

        requirements = holdRequirements[mintTokenId];
        requirementsLength = requirements.length;
        holdIds = new uint256[](requirementsLength);
        holdAmounts = new uint256[](requirementsLength);
        for (uint256 i = 0; i < requirementsLength; i++) {
            holdIds[i] = requirements[i].tokenId;
            holdAmounts[i] = requirements[i].amount;
        }
    }
}