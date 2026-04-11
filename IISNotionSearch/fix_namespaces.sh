#!/bin/bash
# 1. Update TokenHelper & PasswordHelper
sed -i 's/namespace IISNotionSearch.Application.Services.Helpers;/namespace IISNotionSearch.Infrastructure.Security.Helpers;/g' ./IISNotionSearch.Infrastructure/Security/Helpers/*.cs
sed -i 's/using IISNotionSearch.Application.Interfaces.Helpers;/using IISNotionSearch.Application.Common.Interfaces.Security;/g' ./IISNotionSearch.Infrastructure/Security/Helpers/*.cs
# 2. Update ITokenHelper & IPasswordHelper
sed -i 's/namespace IISNotionSearch.Application.Interfaces.Helpers;/namespace IISNotionSearch.Application.Common.Interfaces.Security;/g' ./IISNotionSearch.Application/Common/Interfaces/Security/*.cs
# 3. Update StandardResponse
sed -i 's/namespace IISNotionSearch.Application.Abstractions;/namespace IISNotionSearch.Application.Models;/g' ./IISNotionSearch.Application/Models/StandardResponse.cs
# 4. Update Infrastructure DI
sed -i 's/using IISNotionSearch.Application.Interfaces.Helpers;/using IISNotionSearch.Application.Common.Interfaces.Security;/g' ./IISNotionSearch.Infrastructure/DependencyInjection.cs
sed -i 's/using IISNotionSearch.Application.Services.Helpers;/using IISNotionSearch.Infrastructure.Security.Helpers;/g' ./IISNotionSearch.Infrastructure/DependencyInjection.cs
# 5. Update AuthService
sed -i 's/using IISNotionSearch.Application.Interfaces.Helpers;/using IISNotionSearch.Application.Common.Interfaces.Security;/g' ./IISNotionSearch.Application/Services/AuthService.cs
sed -i 's/using IISNotionSearch.Application.Abstractions;/using IISNotionSearch.Application.Models;/g' ./IISNotionSearch.Application/Services/AuthService.cs
# 6. Update Controllers
sed -i 's/using IISNotionSearch.Application.Abstractions;/using IISNotionSearch.Application.Models;/g' ./IISNotionSearch.API/Controllers/*.cs
sed -i 's/using IISNotionSearch.Application.Abstractions;/using IISNotionSearch.Application.Models;/g' ./IISNotionSearch.API/Abstractions/BaseController.cs
# 7. Update Application DI (remove unused using)
sed -i '/using IISNotionSearch.Application.Abstractions;/d' ./IISNotionSearch.Application/DependencyInjection.cs
sed -i '/using IISNotionSearch.Application.Interfaces.Helpers;/d' ./IISNotionSearch.Application/DependencyInjection.cs
