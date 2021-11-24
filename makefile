export VERSION ?= $(shell gogitver)
export ASSEMBLY_VERSION ?= $(shell echo $(VERSION) | sed -e "s/^v//")
build:
	dotnet restore /property:Version=$(ASSEMBLY_VERSION)
	dotnet publish --configuration Release /property:Version=$(ASSEMBLY_VERSION) --self-contained --runtime osx-x64 --output artifacts/darwin
	dotnet publish --configuration Release /property:Version=$(ASSEMBLY_VERSION) --self-contained --runtime linux-x64 --output artifacts/linux
	dotnet publish --configuration Release /property:Version=$(ASSEMBLY_VERSION) --self-contained --runtime linux-arm --output artifacts/arm
	dotnet publish --configuration Release /property:Version=$(ASSEMBLY_VERSION) --self-contained --runtime win-x64 --output artifacts/windows

package: build
	tar -C artifacts/darwin -zcf artifacts/darwin.tar.gz GtfsXlsxCli
	tar -C artifacts/linux -zcf artifacts/linux.tar.gz GtfsXlsxCli
	tar -C artifacts/arm -zcf artifacts/arm.tar.gz GtfsXlsxCli
	(cd artifacts/windows && zip -r ../windows.zip GtfsXlsxCli.exe)