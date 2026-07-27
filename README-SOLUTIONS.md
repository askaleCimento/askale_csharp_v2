# Çözüm ayrımı

## AskalePortal.Core.sln
.NET 9 projelerini içerir:
- AskalePortal.API
- AskalePortal.BLL
- AskalePortal.Constants
- AskalePortal.Data
- AskalePortal.DAL

DocumentOpen bu solution içinde değildir.

## DocumentOpen.sln
Ayrı ASP.NET MVC 5 / .NET Framework 4.8 web uygulamasıdır.

Gereksinimler:
1. Visual Studio'da .NET Framework 4.8 Developer Pack ve ASP.NET/Web Development workload kurulu olmalı.
2. NuGet restore çalıştırılmalı.
3. Lisanslı GleamTech.Core.dll ve GleamTech.DocumentUltimate.dll dosyaları `DocumentOpen/lib` klasörüne konulmalı.
4. IIS/IIS Express üzerinden çalıştırılmalı.

DocumentOpen endpoint örneği:
`/Document/Open?filename=test.pdf`
