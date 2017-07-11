from os import listdir
from os.path import isfile, join
import matplotlib.pyplot as plt

mypath = r'Graphs\a924e6d7-3811-418a-9022-c8df88738303'
onlyfiles = [f for f in listdir(mypath) if isfile(join(mypath, f))]

for fn in onlyfiles:
	fname = mypath + '\\' + fn
	with open(fname) as f:
		content = f.readlines()
	content = map(list, zip(*[x.strip().split(",")[2:] for x in content][1:]))

	c2 = map(list,zip(*list(content)))
	c2.sort(key=lambda tup: int(tup[0]))
	content = map(list, zip(*list(c2)))
	#print content
	x = content[0]
	y2 = map(list, zip(*content[1:]))
	y2 = map(list, zip(*y2))
	#print y2
	#print len(x)
	#fig, ax1 = plt.subplots()
	fig_size = plt.rcParams["figure.figsize"]
	fig_size[0] = 8
	fig_size[1] = 6
	plt.rcParams["figure.figsize"] = fig_size
	#plt.scatter(x,y2)
	#plt.plot(x, y2, 'o-')#,'bs', 'g^')
	plt.plot(x, y2[0],'bo-.',x, y2[1],'ys-.',x,y2[2],'r^-.',x,y2[3],'md-.', x,y2[4],'gX-.', fillstyle='none')
	plt.title("Vertex count = " + fn.split("_")[1][:-4])
	plt.xlabel('Edge count')
	plt.ylabel('Cost')
	plt.legend(['Greedy VC', 'Round Robin VC', 'Greedy IS', 'Round Robin IS','Round Robin+'], loc='upper left')
	plt.grid()
	#plt.show()
	#break
	plt.savefig(fn + ".png",bbox_inches='tight')
	plt.gcf().clear()
#plt.show()

