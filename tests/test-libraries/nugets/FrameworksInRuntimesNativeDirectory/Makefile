TOP=../../../..
include $(TOP)/Make.config

unexport MSBUILD_EXE_PATH

NuGet.config global.json:
	$(Q) $(MAKE) -C $(TOP)/tests/dotnet $@
	$(Q) $(CP) $(TOP)/tests/dotnet/$@ .

.libs:
	$(Q) mkdir -p $@

PACKAGE_ID=$(shell grep PackageId FrameworksInRuntimesNativeDirectory.csproj | sed 's_.*<PackageId>\(.*\)</PackageId>.*_\1_')
PACKAGE_VERSION=$(shell grep '<PackageVersion>' FrameworksInRuntimesNativeDirectory.csproj | sed 's_.*<PackageVersion>\(.*\)</PackageVersion>.*_\1_')

# Test case for https://github.com/xamarin/xamarin-macios/issues/12440
.libs/FrameworksInRuntimesNativeDirectory.nupkg: FrameworksInRuntimesNativeDirectory.csproj NuGet.config global.json $(wildcard *.cs) | .libs
	$(Q) mkdir -p $(abspath $(NUGET_TEST_FEED))
	$(Q_GEN) $(DOTNET6) pack /bl $(DOTNET_PACK_VERBOSITY) $<
	$(Q) $(CP) bin/Debug/Xamarin.Tests.FrameworksInRuntimesNativeDirectory.$(PACKAGE_VERSION).nupkg $@

INSTALLED_PACKAGE=$(NUGET_TEST_FEED)/xamarin.tests.frameworksinruntimesnativedirectory/$(PACKAGE_VERSION)/xamarin.tests.frameworksinruntimesnativedirectory.$(PACKAGE_VERSION).nupkg

$(INSTALLED_PACKAGE): .libs/FrameworksInRuntimesNativeDirectory.nupkg
	if test -d $(NUGET_TEST_FEED)/$(PACKAGE_ID)/$(PACKAGE_VERSION); then nuget delete $(PACKAGE_ID) $(PACKAGE_VERSION) -source $(abspath $(NUGET_TEST_FEED)) -NonInteractive || true; fi
	rm -Rf $(TOP)/tests/dotnet/packages/xamarin.tests.frameworksinruntimesnativedirectory
	mkdir -p $(abspath $(NUGET_TEST_FEED))
	nuget add "$<" -source $(abspath $(NUGET_TEST_FEED)) -NonInteractive

all-local:: $(INSTALLED_PACKAGE)
