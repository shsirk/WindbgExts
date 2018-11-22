import os, sys, shutil
import argparse

def copy_template(projectname):
    print "[+] Copying template to %s" % projectname
    print "    - Copying clean project"
    currentfolder = os.getcwd()
    projectsource = os.path.join(currentfolder,"WinDbgCSharpPluginTemplate")
    newproject = os.path.join(currentfolder,projectname)
    shutil.copytree(projectsource,newproject)
    
    update_dirs_map = {} 
    print "[+] Updating project files"
    for root, dirs, files in os.walk(newproject):
        if "packages" in dirs:
            dirs.remove("packages")
        for fname in files:
          currentfile = os.path.join(root,fname)
          newfilename = fname.replace("WinDbgCSharpPluginTemplate",projectname)
          print "    - Processing %s -> %s" % (fname,newfilename)
          filecontents = open(currentfile,"rb").read()
          os.remove(currentfile)
          updatedcontents = filecontents.replace("WinDbgCSharpPluginTemplate",projectname)
          objfile = open(os.path.join(root,newfilename),"wb")
          objfile.write(updatedcontents)
          objfile.close()
          
        for dir in dirs:
            if dir == "WinDbgCSharpPluginTemplate":
                update_dirs_map[os.path.join(root, dir)] = os.path.join(root, projectname)
    
    print "[+] Updating directories"
    for old, new in update_dirs_map.items():
        print "    - Processing %s -> %s" % (old,new)
        os.rename(old, new)
            
    print "[+] Done"
    print """
          NOTE:
            You need to go to {0} directory and execute DllExport.bat 
            
            DllExport.bat -action Configure
          
    """.format(newproject)
    
if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="")
    parser.add_argument('-p', '--project', type=str, nargs='?', default="", help='new project name')
    args = parser.parse_args()
    if args.project:
      copy_template(args.project)
