# LegalOffice Clean Architecture MVC (.NET 8)

مشروع MVC لإدارة مكتب محاماة مبني Clean Architecture.

## Structure

```text
src/
├── LegalOffice.Domain          # Entities فقط
├── LegalOffice.Application     # Interfaces + ViewModels
├── LegalOffice.Infrastructure  # EF Core + Identity + Services + Seeder
└── LegalOffice.Web             # MVC Controllers + Views + wwwroot
```

## Features

- ASP.NET Core MVC .NET 8
- Clean Architecture
- EF Core + SQL Server
- ASP.NET Identity Login/Roles
- Clients CRUD
- Lawyers CRUD
- Cases CRUD
- Assign multiple lawyers to one case
- Multiple hearings per case
- Status per hearing via Lookups
- Case Timeline
- Lookups CRUD
- WhatsApp/SMS Mock service ready to replace with Twilio/Meta
- Dashboard

## Run

```bash
cd src/LegalOffice.Web
dotnet restore
dotnet ef migrations add InitialCreate --project ../LegalOffice.Infrastructure --startup-project .
dotnet ef database update --project ../LegalOffice.Infrastructure --startup-project .
dotnet run
```

Default admin:

```text
admin@legal.local
123456
```

## Notes

- غيّر ConnectionStrings:DefaultConnection من appsettings.json.
- خدمة الرسائل الحالية Mock. استبدل `MockMessageService` بـ Twilio أو WhatsApp Cloud API.
- كل القيم المتغيرة موجودة في Lookups: CaseType, CaseStatus, Court, HearingStatus, DocumentType, ExpenseType.
