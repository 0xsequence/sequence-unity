import { ethers, formatEther, parseEther } from 'ethers'
import * as chai from 'chai'
import chaiAsPromised from 'chai-as-promised'

const { expect } = chai.use(chaiAsPromised)

const provider = new ethers.JsonRpcProvider('http://0.0.0.0:8545')
const wallet1 = new ethers.Wallet("0xabc0000000000000000000000000000000000000000000000000000000000001", provider)
const wallet2 = new ethers.Wallet("0xabc0000000000000000000000000000000000000000000000000000000000002", provider)
const signer = wallet1.connect(provider)

const testChainId = async () => {
  const network = await provider.getNetwork()
  console.log("ChainId " + network.chainId)
  expect(network.chainId).to.equal(31337n)
}

const testAddress = async () => {
  const addressExpected1 = "0xc683a014955b75F5ECF991d4502427c8fa1Aa249"
  console.log("Address " + wallet1.address)
  expect(wallet1.address).to.equal(addressExpected1)
}

const sendAmount = '0.1';

const sendBasicTransaction = async () => {
  const currentNonce = await provider.getTransactionCount(wallet1.address);

  const transaction = {
    to: wallet2.address,
    value: parseEther(sendAmount),
    nonce: currentNonce,
  };
  
  try {
    const tx = await signer.sendTransaction(transaction);
    const receipt = await tx.wait();

    console.log(receipt);

    return {tx, receipt};
  }
  catch (error) {
    console.log(error);
    return {error};
  }
}

const testBalanceChangesAfterTransaction = async () => {
  const balance1Before = await provider.getBalance(wallet1.address)
  console.log("Balance 1 before " + formatEther(balance1Before))
  const balance2Before = await provider.getBalance(wallet2.address)
  console.log("Balance 2 before " + formatEther(balance2Before))

  await sendBasicTransaction()

  const balance1After = await provider.getBalance(wallet1.address)
  console.log("Balance 1 after " + formatEther(balance1After))
  const balance2After = await provider.getBalance(wallet2.address)
  console.log("Balance 2 after " + formatEther(balance2After))

  expect(balance1After < (balance1Before - parseEther(sendAmount))) // Should be slightly less due to gas fees
  expect(balance2After).to.equal(balance2Before + parseEther(sendAmount))
}

const testTransactionReceiptMethod = async () => {
  const {tx, receipt, error} = await sendBasicTransaction()
  expect(error).to.be.undefined
  expect(receipt).to.not.be.undefined
  expect(tx).to.not.be.undefined

  const fetchedReceipt = await provider.getTransactionReceipt(tx!.hash)
  expect(fetchedReceipt).to.not.be.undefined

  expect(receipt!.blockHash).to.equal(fetchedReceipt!.blockHash)
  expect(receipt!.hash).to.equal(fetchedReceipt!.hash)
}


const runTests = async () => {
  testChainId()
  testAddress()
  await testBalanceChangesAfterTransaction()
  await testTransactionReceiptMethod()
}

runTests()