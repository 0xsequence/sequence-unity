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
	cd ./testchain && pnpm install

start-testchain:
	cd ./testchain && pnpm start:hardhat 

start-testchain-verbose:
	cd ./testchain && pnpm start:hardhat:verbose

start-testchain-geth:
	cd ./testchain && pnpm start:geth

start-testchain-geth-verbose:
	cd ./testchain && pnpm start:geth:verbose

check-testchain-running:
	@curl http://localhost:8545 -H"Content-type: application/json" -X POST -d '{"jsonrpc":"2.0","method":"eth_syncing","params":[],"id":1}' --write-out '%{http_code}' --silent --output /dev/null | grep 200 > /dev/null \
	|| { echo "*****"; echo "Oops! testchain is not running. Please run 'make start-testchain' in another terminal or use 'test-concurrently'."; echo "*****"; exit 1; }

test-testchain: 
	cd ./testchain && (pnpm start:hardhat & echo $$! > .pid) && pnpm test > ../chaintest.out && cd .. && make stop && cat chaintest.out

stop:
	-pkill -F ./testchain/.pid && rm testchain/.pid

test:
	rm TestResults*.xml ; cd ./testchain && (pnpm start:hardhat & echo $$! > .pid) && cd .. && \
	Unity -batchmode -runTests -projectPath "$(pwd)" -testPlatform editmode -testResults TestResults_Edit.xml ; Unity -quit && \
	Unity -runTests -projectPath "$(pwd)" -testPlatform playmode -testResults TestResults_Play.xml ; \
	make stop && \
	echo "Edit mode Test results: " && \
	head -n 2 TestResults_Edit.xml | grep -Eo 'result="[^"]+"|total="[^"]+"|passed="[^"]+"|failed="[^"]+"|inconclusive="[^"]+"|skipped="[^"]+"|start-time="[^"]+"|end-time="[^"]+"|duration="[^"]+"' | grep -Ev 'clr-version=|engine-version=|asserts=|id=|testcasecount=' | sed -E 's/^[^"]+"([^"]+)"[^"]+"([^"]+)".*/\1: \2/' && \
	echo "" && \
	echo "Play mode Test results: " && \
	head -n 2 TestResults_Play.xml | grep -Eo 'result="[^"]+"|total="[^"]+"|passed="[^"]+"|failed="[^"]+"|inconclusive="[^"]+"|skipped="[^"]+"|start-time="[^"]+"|end-time="[^"]+"|duration="[^"]+"' | grep -Ev 'clr-version=|engine-version=|asserts=|id=|testcasecount=' | sed -E 's/^[^"]+"([^"]+)"[^"]+"([^"]+)".*/\1: \2/'

test-ui:
	rm TestResults*.xml ; \
	Unity -runTests -projectPath "$(pwd)" -testPlatform playmode -testResults TestResults_Play.xml ; \
	echo "Play mode Test results: " && \
	head -n 2 TestResults_Play.xml | grep -Eo 'result="[^"]+"|total="[^"]+"|passed="[^"]+"|failed="[^"]+"|inconclusive="[^"]+"|skipped="[^"]+"|start-time="[^"]+"|end-time="[^"]+"|duration="[^"]+"' | grep -Ev 'clr-version=|engine-version=|asserts=|id=|testcasecount=' | sed -E 's/^[^"]+"([^"]+)"[^"]+"([^"]+)".*/\1: \2/'

test-sdk:
	rm TestResults*.xml ; cd ./testchain && (pnpm start:hardhat & echo $$! > .pid) && cd .. && \
	Unity -batchmode -runTests -projectPath "$(pwd)" -testPlatform editmode -testResults TestResults_Edit.xml ; Unity -quit && \
	make stop && \
	echo "Edit mode Test results: " && \
	head -n 2 TestResults_Edit.xml | grep -Eo 'result="[^"]+"|total="[^"]+"|passed="[^"]+"|failed="[^"]+"|inconclusive="[^"]+"|skipped="[^"]+"|start-time="[^"]+"|end-time="[^"]+"|duration="[^"]+"' | grep -Ev 'clr-version=|engine-version=|asserts=|id=|testcasecount=' | sed -E 's/^[^"]+"([^"]+)"[^"]+"([^"]+)".*/\1: \2/'
