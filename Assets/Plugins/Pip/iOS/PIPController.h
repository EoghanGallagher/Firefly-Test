#import <PIPSDK/PIPEvents.h>
#import <PIPSDK/PipManager.h>

@interface PipController : NSObject <PipAnalyzerProtocol, PipManagerProtocol, PipConnectionProtocol, PipControlProtocol>
{
}

+ (PipController *) sharedManager;

- (bool) isBluetoothActive;
- (void) discoverPips;
- (void) cancelDiscovery;
- (bool) isDiscovering;
- (void) suspendPips;
- (void) resumePips;
- (void) resetAll;
- (const int) getNumDiscoveries;
- (const char*) getNameForDiscoveredPip:(int) index;
- (const int) getPipIDForDiscoveredPip:(int) index;
- (const int) getNumRegistered;
- (const char*) getNameForRegisteredPip:(int) index;
- (const int) getPipIDForRegisteredPip:(int) index;
- (void) registerPip:(int) pipID;
- (void) unregisterPip:(int)pipID;
- (void) unregisterAll;

- (const char*) getAllowedPipNameChars;
- (void) connectPip:(PipID) pipID;
- (void) disconnectPip:(PipID) pipID;
- (bool) isPipConnected:(PipID) pipID;
- (bool) isPipActive:(PipID) pipID;

- (const char*) getName:(PipID) pipID;
- (const char*) getVersion:(PipID) pipID;
- (int) getBatteryLevel:(PipID) pipID;
- (int) getBatteryEvent:(PipID) pipID;

- (void) requestVersion:(PipID) pipID;
- (void) requestGetName:(PipID) pipID;
- (void) requestSetName:(PipID) pipID name:(const char*)newName;
- (void) requestReset:(PipID) pipID;
- (void) requestPowerOff:(PipID) pipID;
- (void) requestPing:(PipID) pipID  flashLED:(bool)flash;
- (void) startStreaming:(PipID) pipID;
- (void) stopStreaming:(PipID) pipID;
- (bool) isStreaming:(PipID) pipID;

@end
