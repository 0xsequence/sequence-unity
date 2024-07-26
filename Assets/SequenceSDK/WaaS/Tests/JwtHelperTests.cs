using NUnit.Framework;
using Sequence;
using Sequence.EmbeddedWallet;

namespace Sequence.EmbeddedWallet.Tests
{
    public class JwtHelperTests
    {
        [TestCase("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJwYXJ0bmVyX2lkIjoyLCJ3YWxsZXQiOiIweDY2MDI1MDczNGYzMTY0NDY4MWFlMzJkMDViZDdlOGUyOWZlYTI5ZTEifQ.FC8WmaC_hW4svdrs4rxyKcvoekfVYFkFFvGwUOXzcHA", "0x660250734f31644681ae32d05bd7e8e29fea29e1")]
        [TestCase("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJwYXJ0bmVyX2lkIjoxLCJ3YWxsZXQiOiIweGE2MDI1MDczNGYzMTY0NDY4MWFlMzJkMDViZDdlOGUyOWZlYTI5ZTEifQ.SgaUaoHJhKwYTNWdMKlKRC_Hj27Kqovv3qI7Ky-TBkg", "0xa60250734f31644681ae32d05bd7e8e29fea29e1")]
        [TestCase("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJwYXJ0bmVyX2lkIjoxMDAwMDAsIndhbGxldCI6IjB4YTYwMjUwNzM0ZjMxNjQ0NjgxYWUzMmQwNWJkN2U4ZTI5ZmVhMjllMSJ9.CmBaisrov_AST9tgtfDcO58lh2t63CxAcyd0SUCYIJc", "0xa60250734f31644681ae32d05bd7e8e29fea29e1")]
        [TestCase("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJwYXJ0bmVyX2lkIjoyLCJ3YWxsZXQiOiIweDY2MDI1MDczNGYzMTY0NDY4MWFlMzJkMDViZDdlOGUyOWZlYWFhZTEifQ.IAT79FXnsMreAEuC7eGyaW3X-xWnL6Vul9qA1SBU7eA", "0x660250734f31644681ae32d05bd7e8e29feaaae1")]
        [TestCase("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJwYXJ0bmVyX2lkIjoxMDAwMDAwMDAwMDAwMDAwMDAsIndhbGxldCI6IjB4NjYwMjUwNzM0ZjMxNjQ0NjgxYWUzMmQwNWJkN2U4ZTI5ZmVhMjllMSJ9.CGAzQj-qT2mwuJ8Z6h9dMf1ubh7bAf17iBH3xRzmLnk", "0x660250734f31644681ae32d05bd7e8e29fea29e1")]
        public void TestGetWalletAddressFromJwt(string jwt, string expected)
        {
            Address result = JwtHelper.GetWalletAddressFromJwt(jwt);
            Assert.AreEqual(expected, result.Value);
        }
    }
}