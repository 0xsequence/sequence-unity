pragma solidity ^0.8.0;

contract TestContractWithTuples {

    struct SingleTuple {
        address someAddress;
        string first;
    }

    struct TupleArray {
        string second;
        address another;
        bytes data;
        uint number;
    }

    address public storedAddress;
    address public storedAddress2;
    string public storedString;
    string public storedString2;
    uint public storedUint;
    int public storedInt;
    bytes public storedBytes;
    function testTuple(SingleTuple memory singleTuple, TupleArray[] memory tupleArray, int integer) public {
        storedAddress = singleTuple.someAddress;
        storedString = singleTuple.first;
        storedInt = integer;
        if (tupleArray.length > 0) {
            storedAddress2 = tupleArray[0].another;
            storedString2 = tupleArray[0].second;
            storedBytes = tupleArray[0].data;
            storedUint = tupleArray[0].number;
        }
    }

    function getStoredAddress() public view returns (address) {
        return storedAddress;
    }

    function getStoredAddress2() public view returns (address) {
        return storedAddress2;
    }

    function getStoredString() public view returns (string memory) {
        return storedString;
    }

    function getStoredString2() public view returns (string memory) {
        return storedString2;
    }

    function getStoredUint() public view returns (uint) {
        return storedUint;
    }

    function getStoredInt() public view returns (int) {
        return storedInt;
    }

    function getStoredBytes() public view returns (bytes memory) {
        return storedBytes;
    }
}