pragma solidity ^0.8.9;

contract TestEncodingAndDecoding {
    function testPart1(
        uint256 number,
        string memory word,
        uint[] memory numbers,
        string[][] memory doubleNestedWords
    ) public pure returns (uint256, string memory, uint[] memory, string[][] memory) {
        return (number, word, numbers, doubleNestedWords);
    }

    function testPart2(
        bool boolean,
        bool[] memory bools,
        bytes memory byteArray,
        bytes32 moreBytes,
        address[] memory addresses
    ) public pure returns (bool, bool[] memory, bytes memory, bytes32, address[] memory) {
        return (boolean, bools, byteArray, moreBytes, addresses);
    }
}