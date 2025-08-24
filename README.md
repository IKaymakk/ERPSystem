# ERP System (Henüz Tamamlanmadı)

## Proje Hakkında
Bu proje, bir ERP (Enterprise Resource Planning) sisteminin geliştirilmesine yönelik bir çalışma olarak tasarlanmıştır. Sistem, modüler bir yapıda planlanmış ve her modül kendi içerisinde Entity → Service → API → UI sırasıyla geliştirilmiştir.

Projede kullanılan başlıca teknolojiler:
- Backend: .NET 8, ASP.NET Core, ASP.NET Web API, Entity Framework
- Frontend: Angular veya React (En Son Aşama)
- Database: MSSQL, Redis
- Authentication: JWT, Role-based permissions
- Logging & Monitoring: Serilog, Exception Middleware

## Modüller ve Yol Haritası

### 1. Kullanıcı Yönetimi (Auth & Authorization)
- DbContext kurulumu ve Configuration
- Repository Pattern (Generic Repository + Unit of Work)
- AutoMapper profilleri
- JWT Authentication
- CRUD işlemleri, Login/Logout, Role-based permissions

### 2. Stok Yönetimi
- Kategori ve Ürün Yönetimi
- Stok hareketleri (Giriş/Çıkış/Adjust)
- Stok raporları ve düşük stok uyarıları
- Ürün görsel yükleme ve barkod yönetimi

### 3. Müşteri / Tedarikçi Yönetimi
- Company ve Contact Management
- Kredi limiti ve bakiye hesaplamaları
- API ve ön yüz (customer/supplier) yönetimi

### 4. Satış Yönetimi
- Satış siparişleri ve fatura yönetimi
- Stok güncelleme, kredi limiti kontrolü
- API ve ön yüz dashboardları, PDF çıktılar

### 5. Satın Alma Yönetimi
- Satın alma siparişleri ve alım takip
- Kısmi alım ve stok güncellemeleri
- API ve UI arayüzleri

### 6. Ödeme Yönetimi
- Ödeme kayıtları ve eşleştirme
- Çoklu ödeme desteği
- Finansal dashboard ve raporlar

### 7. Ek Geliştirmeler
- Logging & Monitoring
- Unit & Integration Testler
- MediatR ile CQRS pattern
- Redis veya In-Memory caching
- Background jobs (Hangfire)
- E-posta bildirimleri, PDF ve Excel export/import


