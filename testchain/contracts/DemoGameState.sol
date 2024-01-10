// SPDX-License-Identifier: MIT
pragma solidity ^0.8.9;

import "@openzeppelin/contracts/token/ERC1155/ERC1155.sol";

contract DemoGameState is ERC1155 {

    address public owner;
    bool public on;

    modifier onlyOwner(){
        require(msg.sender == owner);
        _;
    }

    constructor() ERC1155("") {
        owner = msg.sender;
        on = true;
    }

    function setURI(string memory newuri) public onlyOwner {
        _setURI(newuri);
    }

    function mint(uint256 id, uint256 amount)
        public
    {
        require(on, "DemoGameState: minting is off");
        _mint(msg.sender, id, amount, "");
    }

    function mintBatch(address to, uint256[] memory ids, uint256[] memory amounts, bytes memory data)
        public
        onlyOwner
    {
        _mintBatch(to, ids, amounts, data);
    }

    function updateOwner(address _newOwner) onlyOwner external {
        owner = _newOwner;
    }

    function toggle(bool _state) onlyOwner external {
        on = _state;
    }
}
