# NUnit3TestAdapterMismatchRepro
A small repro of how to fail to load .NET Framework NUnit tests when mistakenly using a .NET Core version of NUnit3TestAdapter, when the TRX logger option is used.

To run the repro:
* Build the solution
* Open a "Developer Command Prompt for VS 2019" in the repository root directory
* Run the repro.cmd script
