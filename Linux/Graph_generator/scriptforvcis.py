import os
from os import walk
inPref = "Data"
outPref = "OutVCIS"
dirPath = "/home/deepika/Desktop/Graph_generator/" + inPref
f = []
for (dirpath, dirnames, filenames) in walk(dirPath):
    f.extend(filenames)
    break

for fn in f:
    if((os.path.isfile(outPref + "/" + fn + ".output") == False) and fn.endswith(".input")):
        cmd = "/home/deepika/Desktop/NuMVCcode/numvc " + inPref + "/" + fn + " 0 1 2 > " + outPref + "/" + fn + ".output.vi"
        print cmd
        os.system(cmd)
        #cmd = "/home/deepika/Desktop/NuMVCcode/numvc" + wPref + " " + inPref + "/" + fn + " 0 1 2 R > " + outPrefRR + "/" + fn + ".output"
        #print cmd
        #os.system(cmd)
    else:
        print "this file already there:" + fn + ".output"
    


