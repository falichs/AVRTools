import requests

#xml = '<?xml version="1.0" encoding="utf-8" ?> \
#<tx> \
#<cmd id="1">GetAllZonePowerStatus</cmd> \
#</tx>'
#headers = {'Content-Type': 'application/xml'} # set what your server accepts
#r = requests.post('http://192.168.0.10:80/goform/AppCommand.xml', data=xml, headers=headers)
#print(r.text)

#POWER OFF
#r=requests.get("http://192.168.0.10:80/goform/formiPhoneAppDirect.xml?PWSTANDBY")

#POWER ON
#r=requests.get("http://192.168.0.10:80/goform/formiPhoneAppDirect.xml?PWON")

#POWER STATUS
r=requests.get("http://192.168.0.10:80/goform/formiPhoneAppDirect.xml?PWSTAT")

#r=requests.get("http://192.168.0.10:80/goform/formiPhoneAppPower.xml?1+PowerOn")

print(r.text)
