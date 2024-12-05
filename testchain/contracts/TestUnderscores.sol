pragma solidity ^0.8.9;

contract TestUnderscores {
    function test_function_name_with_underscores(
        uint256 number_with_underscore
    ) public returns (uint256) {
        return (number_with_underscore);
    }
}