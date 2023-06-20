import { ethers } from 'ethers'

const provider = new ethers.JsonRpcProvider('http://0.0.0.0:8545')

const testChainId = async () => {
  const network = await provider.getNetwork()
  console.log(network.chainId)
}

testChainId()
