# updates the copyright information for all .cs files
# usage: call recursive_traversal, with the following parameters
# parent directory, old copyright text content, new copyright text content

import os

excludedir = ["..\\Lib"]

def update_source(filename, oldcopyright, copyright):
    utfstr = chr(0xef)+chr(0xbb)+chr(0xbf)
    fdata = open(filename,"r+").read()
    isUTF = False
    if (fdata.startswith(utfstr)):
        isUTF = True
        fdata = fdata[3:]
    if (oldcopyright != None):
        if (fdata.startswith(oldcopyright)):
            fdata = fdata[len(oldcopyright):]
    if not (fdata.startswith(copyright)):
        print ("updating "+filename)
        fdata = copyright + fdata
        if (isUTF):
            open(filename,"w").write(utfstr+fdata)
        else:
            open(filename,"w").write(fdata)

def recursive_traversal(dir,  oldcopyright, copyright):
    global excludedir
    fns = os.listdir(dir)
    print ("listing "+dir)
    for fn in fns:
        fullfn = os.path.join(dir,fn)
        if (fullfn in excludedir):
            continue
        if (os.path.isdir(fullfn)):
            recursive_traversal(fullfn, oldcopyright, copyright)
        else:
            if (fullfn.endswith(".cs")):
                update_source(fullfn, oldcopyright, copyright)
    
     
oldcright = open("Copyright.txt","r+").read()
cright = open("Copyright.txt","r+").read()
recursive_traversal(os.path.dirname(os.path.realpath(__file__)), oldcright, cright)
exit()