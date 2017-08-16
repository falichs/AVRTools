import socket
import http.client
import io
import sys
import logging
import requests

class SSDPResponse():
    """A simple network discovery class"""
    class _FakeSocket(io.BytesIO):
        def makefile(self, *args, **kw):
            return self
    def __init__(self, response):
        r = http.client.HTTPResponse(self._FakeSocket(response))
        r.begin()
        self.location = r.getheader("location")
        self.usn = r.getheader("usn")
        self.st = r.getheader("st")
        self.cache = r.getheader("cache-control").split("=")[1]
    def __repr__(self):
        return "<SSDPResponse({location}, {st}, {usn})>".format(**self.__dict__)

def discover(service, timeout=5, retries=1, mx=3):
    group = ("239.255.255.250", 1900)
    message = "\r\n".join([
        'M-SEARCH * HTTP/1.1',
        'HOST: {0}:{1}',
        'MAN: "ssdp:discover"',
        'ST: {st}','MX: {mx}','',''])
    socket.setdefaulttimeout(timeout)
    responses = {}
    for _ in range(retries):
        sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM, socket.IPPROTO_UDP)
        sock.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        sock.setsockopt(socket.IPPROTO_IP, socket.IP_MULTICAST_TTL, 2)
        message_bytes = message.format(*group, st = service, mx=mx).encode('utf-8')
        sock.sendto(message_bytes, group)

        while True:
            try:
                response = SSDPResponse(sock.recv(1024))
                responses[response.location] = response
            except socket.timeout:
                break
    return list(responses.values())


logging.basicConfig(filename='ssdpDiscover.log', format='%(levelname)s:%(message)s', filemode='w',  level=logging.DEBUG)
logging.info(sys.executable)
logging.info(sys.version)
logging.info("Scanning for SSDP Services...")
hosts = discover("ssdp:all")
# hosts = discover("urn:schemas-upnp-org:service:AVTransport:1")
for h in hosts:
    logging.info("Service found:")
    logging.info('\t' + h.location)
    logging.info('\t' + h.st)
    logging.info('\t' + h.usn)
    logging.info('\t' + h.cache)
    r=requests.get(h.location.strip())
    logging.info(r.text)
