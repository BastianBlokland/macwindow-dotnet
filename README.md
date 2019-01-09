# [Retired] Netstandard2.0 c# bindings around native macOS window management

Still very work in progress. Can be used to create windows from a pure netcoreapp, started this project to created a c# vulkan renderer (using one of the available vulkan c# bindings and using moltenVK)

The c# side is dynamically linked to the 'libmacwindow.dylib', which can be build from the xcode project in 'native-src'. The native side uses simple c-style extern methods and uses objective c where needed to interop with the cocoa framework.
