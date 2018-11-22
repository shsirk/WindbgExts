
WinDbg C# extension template

Derived from 
    https://blogs.msdn.microsoft.com/rodneyviana/2016/05/18/windbg-extension-written-completely-in-c/ 

FIX: 
  - updated to use .NET Export (https://github.com/3F/DllExport)
  - stripped down code to minimal template only format. 
  - to use local debugger interface (Debugger)
  
  
Build:
  Visual studio community 2017

Used Against
  WinDbg Preview 
  
NOTE: 
    DllExport action Configure needs to be run to build x86 / x86 seperately (see usage https://github.com/3F/DllExport)
    setting platform to x86+x64 didn't work.
    
    