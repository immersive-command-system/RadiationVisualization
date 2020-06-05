# Usage: make generate -j2 
# This runs the position and radiation targets simultaneously and runs cloud when position is complete. Not sure if I need to wait until Radiaiton is complete as well...

generate: position radiation cloud

position:
	python GeneratePosData.py --HOST LOCALHOST --PORT 50007

radiation:
	python GenerateRadiation.py --HOST LOCALHOST --PORT 50009

cloud:
	python GenerateCloud.py --HOST LOCALHOST --PORT 50008
