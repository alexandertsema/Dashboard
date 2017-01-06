using System;

namespace Dashboard.Server.WebSocket
{
    public struct OpCodes
    {
        public static Int16 RequestInfoModel = 1;
        public static Int16 StartBroadcasting = 2;
        public static Int16 StopBroadcasting = 3;
    }
}