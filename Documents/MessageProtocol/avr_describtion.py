import requests

r=requests.get("http://192.168.0.23:80/goform/formiPhoneAppControlJudge.xml")
f = open('avr_goform_formiPhoneAppControlJudge.xml', 'w')
f.write(r.text)
f.close()

r=requests.get("http://192.168.0.23:80/goform/DeviceInfo.xml")
f = open('avr_goform_DeviceInfo.xml', 'w')
f.write(r.text)
f.close()


r=requests.get("http://192.168.0.23:80/goform/formMainZone_MainZoneXml")
f = open('avr_goform_formMainZone_MainZoneXml.xml', 'w')
f.write(r.text)
f.close()

r=requests.get("http://192.168.0.23:80/goform/formMainZone_MainZoneXmlStatus")
f = open('avr_goform_formMainZone_MainZoneXmlStatus.xml', 'w')
f.write(r.text)
f.close()

r=requests.get("http://192.168.0.23:80/goform/formMainZone_MainZoneXmlStatusLite.xml")
f = open('avr_goform_formMainZone_MainZoneXmlStatusLite.xml', 'w')
f.write(r.text)
f.close()

r=requests.get("http://192.168.0.23:80/goform/formZone2_Zone2XmlStatusLite.xml")
f = open('avr_goform_formZone2_Zone2XmlStatusLite.xml', 'w')
f.write(r.text)
f.close()
