import requests

r=requests.get("http://192.168.0.10:80/goform/formiPhoneAppControlJudge.xml")
f = open('avr_goform_formiPhoneAppControlJudge.xml', 'w')
f.write(r.text)
f.close()

r=requests.get("http://192.168.0.10:80/goform/DeviceInfo.xml")
f = open('avr_goform_DeviceInfo.xml', 'w')
f.write(r.text)
f.close()


r=requests.get("http://192.168.0.10:80/goform/formMainZone_MainZoneXml.xml")
f = open('avr_goform_formMainZone_MainZoneXml.xml', 'w')
f.write(r.text)
f.close()

r=requests.get("http://192.168.0.10:80/goform/formMainZone_MainZoneXmlStatus.xml")
f = open('avr_goform_formMainZone_MainZoneXmlStatus.xml', 'w')
f.write(r.text)
f.close()

r=requests.get("http://192.168.0.10:80/goform/formMainZone_MainZoneXmlStatusLite.xml")
f = open('avr_goform_formMainZone_MainZoneXmlStatusLite.xml', 'w')
f.write(r.text)
f.close()

r=requests.get("http://192.168.0.10:80/goform/formZone2_Zone2XmlStatusLite.xml")
f = open('avr_goform_formZone2_Zone2XmlStatusLite.xml', 'w')
f.write(r.text)
f.close()

r=requests.get("http://192.168.0.10:80/goform/formZone2_Zone2XmlStatus.xml")
f = open('avr_goform_formZone2_Zone2XmlStatus.xml', 'w')
f.write(r.text)
f.close()

xml = '<?xml version="1.0" encoding="utf-8" ?> \
<tx> \
<cmd id="1">GetAllZonePowerStatus</cmd> \
</tx>'

headers = {'Content-Type': 'application/xml'}
r = requests.post('http://192.168.0.10:80/goform/AppCommand.xml', data=xml, headers=headers)

f = open('avr_goform_AppCommand.xml', 'w')
f.write(r.text)
f.close()
