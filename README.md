DCCreative.NancyFXOWINSSL
=========================

TestStub for issue with NancyFX, OWIN, SSL and HTTPS redirect

This teststub assumes the following;

* Certificate installed

Note that on Windows hosts a HttpListenerException may be thrown with an Access Denied message. To resolve this the URL has to be added to the ACL.

On Windows Vista/Server 2008 or later, execute the following in PowerShell or CMD running as administrator:

    netsh http add urlacl url=http://+:9100/ user=DOMAIN\username
    netsh http add urlacl url=http://+:9443/ user=DOMAIN\username

Then...

    netsh http add sslcert ipport=0.0.0.0:9443 certhash=<YOURCERTHASH> appid={00112222-4455-6677-8899-AABBCCDDEEFF}.
