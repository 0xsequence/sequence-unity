SHELL             = bash -o pipefail

GITTAG            ?= $(shell git describe --exact-match --tags HEAD 2>/dev/null || :)
GITBRANCH         ?= $(shell git rev-parse --abbrev-ref HEAD 2>/dev/null || :)
LONGVERSION       ?= $(shell git describe --tags --long --abbrev=8 --always HEAD)$(echo -$GITBRANCH | tr / - | grep -v '\-master' || :)
VERSION           ?= $(if $(GITTAG),$(GITTAG),$(LONGVERSION))
GITCOMMIT         ?= $(shell git log -1 --date=iso --pretty=format:%H)
GITCOMMITDATE     ?= $(shell git log -1 --date=iso --pretty=format:%cd)


all:
	@echo "make <cmd>"
	@echo ""
	@echo "commands:"
	@echo ""
	@echo "  - bootstrap"
	@echo "  - start-testchain"
	@echo "  - start-testchain-verbose"
	@echo ""


#
# Testchain
#
bootstrap:
	cd ./testchain && yarn install

start-testchain:
	cd ./testchain && yarn start:hardhat

start-testchain-verbose:
	cd ./testchain && yarn start:hardhat:verbose

start-testchain-geth:
	cd ./testchain && yarn start:geth

start-testchain-geth-verbose:
	cd ./testchain && yarn start:geth:verbose

check-testchain-running:
	@curl http://localhost:8545 -H"Content-type: application/json" -X POST -d '{"jsonrpc":"2.0","method":"eth_syncing","params":[],"id":1}' --write-out '%{http_code}' --silent --output /dev/null | grep 200 > /dev/null \
	|| { echo "*****"; echo "Oops! testchain is not running. Please run 'make start-testchain' in another terminal or use 'test-concurrently'."; echo "*****"; exit 1; }
