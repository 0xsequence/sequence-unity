using NUnit.Framework;

namespace Sequence.Pay.Tests.Transak
{
    public class SequenceTransakContractIdRepositoryValidator
    {
        [Test]
        public void ValidateSequenceTransakContractIdRepositoryIsCorrectlyAssembled()
        {
            foreach (var chain in Pay.Transak.SequenceTransakContractIdRepository.SequenceContractIds.Keys)
            {
                foreach (var orderbookKind in Pay.Transak.SequenceTransakContractIdRepository.SequenceContractIds[chain].Keys)
                {
                    var contractId = Pay.Transak.SequenceTransakContractIdRepository.SequenceContractIds[chain][orderbookKind];
                    Assert.IsNotNull(contractId.Id);
                    Assert.IsNotNull(contractId.ContractAddress);
                    Assert.IsNotNull(contractId.Chain);
                    Assert.AreEqual(chain, contractId.Chain);
                    Assert.IsNotNull(contractId.PriceTokenSymbol);
                }
            }
        }
    }
}