using Microsoft.AspNetCore.Mvc;

namespace ServerApp.Controllers {

    public class ScanRFIDResult {
        public string? Status { get; set; }
        public string? Message { get; set; }
    }

    [ApiController]
    [Route( "api/scan/{rfid}" )]
    public class ScanRFID : Controller {
        [HttpGet]
        public ScanRFIDResult Scan( string rfid ) {
            ScanRFIDResult scanResult = new ScanRFIDResult();
            scanResult.Status = VerifyRFIDTag( rfid ) ? "Success" : "Failed";
            scanResult.Message = "RFID Scanned: " + rfid;
            return scanResult;
        }

        bool VerifyRFIDTag( string rfid ) {
            if ( rfid == "0A 48 3A 29" ) {
                return true;
            }
            else if ( rfid == "99 F7 5F B3" ) {
                return false;
            }
            else {
                return false;
            }
        }
    }

    [ApiController]
    [Route( "api/[controller]" )]
    public class MessageController : ControllerBase {
        private readonly StateWebSocketManager _webSocketManager;

        public MessageController( StateWebSocketManager webSocketManager ) {
            _webSocketManager = webSocketManager;
        }

        [HttpGet( "{clientId}" )]
        public async Task<IActionResult> SendMessage( string clientId, string message ) {
            await _webSocketManager.SendMessageAsync( clientId, message );
            return Ok();
        }
    }

}
