using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace HealthConnectLibraly;
internal class HealthPermissions : BasePlatformPermission
{
#if ANDROID
    public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
            new List<(string permission, bool isRuntime)>
            {
                ("android.permission.health.READ_HYDRATION", true),
                ("android.permission.health.READ_WEIGHT", true),
                ("android.permission.health.WRITE_HYDRATION", true),
                ("android.permission.health.WRITE_WEIGHT", true),
            }.ToArray();
#endif
}
