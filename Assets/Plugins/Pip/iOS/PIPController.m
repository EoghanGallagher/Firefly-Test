#import "PipController.h"
#import <PIPSDK/AnalyzerOutput.h>

void UnitySendMessage( const char* className, const char* methodName, const char* param );

@interface PipController ()
{
}
    @property PipManager *pipManager;
@end

static const int PIP_RESULT_SUCCESS = 0;

@implementation PipController

+ (PipController *) sharedManager
{
    static dispatch_once_t once;
    static id sharedManager;
    dispatch_once(&once, ^{
        sharedManager = [[self alloc] initPipController];
    });
    return sharedManager;
}

- (id) initPipController
{
    NSLog(@"NSLog::PipController::InitPipController");
    self = [super init];
    
    if(self)
    {
        self.pipManager = [PipManager sharedManagerWithDelegate:self];
    }

    return self;
}

- (bool) isBluetoothActive
{
	BOOL active = [self.pipManager isBluetoothAdapterActive];
	return (active == NO ? false : true);
}

- (void) discoverPips
{
	[self.pipManager discoverPips];
}

- (void) cancelDiscovery
{
	[self.pipManager cancelDiscovery];
}

- (bool) isDiscovering
{
	BOOL discovering = [self.pipManager discovering];
	return ( discovering == NO ? false : true);
}

- (void) suspendPips
{
    [self.pipManager suspendPips];
}

- (void) resumePips
{
    [self.pipManager resumePips];
}

- (void) resetAll
{
	[self.pipManager disconnectAllAndReset];
}

- (const int) getNumDiscoveries
{
	return (int)[self.pipManager numDiscoveries]; 
}

- (const char*) getNameForDiscoveredPip:(int) index
{
	return [[[self.pipManager getDiscoveryAtIndex:index] name] UTF8String];
}

- (const int) getPipIDForDiscoveredPip:(int) index
{
	return [[self. pipManager getDiscoveryAtIndex:index] pipID] ;
}

- (const int) getNumRegistered
{
	return (int)[self.pipManager numRegistered];	
}

- (const char*) getNameForRegisteredPip:(int) index
{
	return [[[self.pipManager getRegisteredPipAtIndex:index] name] UTF8String];
}

- (const int) getPipIDForRegisteredPip:(int) index
{
	return (int)[[self.pipManager getRegisteredPipAtIndex:index] pipID] ;
}

- (void) registerPip:(int)pipID
{
	[self.pipManager registerPipWithPipID:pipID];
}

- (void) unregisterPip:(int)pipID
{
	[self.pipManager unregisterPipWithPipID:pipID];
}

- (void) unregisterAll
{
	[self.pipManager unregisterAllPips];
}

- (const char*) getAllowedPipNameChars
{
	return [[Pip allowedNameCharsAsString] UTF8String];
}

- (void) connectPip:(PipID) pipID
{
    [[self.pipManager getPipWithPipID:pipID] setConnectionDelegate:self];
    [[self.pipManager getPipWithPipID:pipID] setControlDelegate:self];
    [[self.pipManager getPipWithPipID:pipID] setAnalyzerDelegate:self];

    [self.pipManager connectPipWithPipID:pipID];
}

- (void) disconnectPip:(PipID) pipID
{
     [self.pipManager disconnectPipWithPipID:pipID];
}

- (bool) isPipConnected:(PipID) pipID
{
	BOOL connected = [[self.pipManager getPipWithPipID:pipID] connected];
	return (connected == NO ? false : true);
}

- (bool) isPipActive:(PipID) pipID
{
	BOOL active = [[self.pipManager getPipWithPipID:pipID] active];
	return (active == NO ? false : true);
}

- (int) getBatteryLevel:(PipID) pipID
{
	return  (int)[[self.pipManager getPipWithPipID:pipID] batteryLevel];
}

- (int) getBatteryEvent:(PipID) pipID
{
	return  (int)[[self.pipManager getPipWithPipID:pipID] batteryEvent];
}

- (const char*) getName:(PipID) pipID
{
    return [[[[self.pipManager getPipWithPipID:pipID] info] name] UTF8String];
}

- (const char*) getVersion:(PipID) pipID
{
	   return [[[self.pipManager getPipWithPipID:pipID] version] UTF8String];
}
		
- (void) requestVersion:(PipID) pipID
{
	[[self.pipManager getPipWithPipID:pipID] requestVersion];
}

- (void) requestGetName:(PipID) pipID
{
	[[self.pipManager getPipWithPipID:pipID] requestName];
}

- (void) requestSetName:(PipID) pipID name:(const char*)newName
{
    NSString* n = [NSString stringWithFormat:@"%s", newName];
    [[self.pipManager getPipWithPipID:pipID] requestSetName:n];
}

- (void) requestPing:(PipID) pipID flashLED:(bool)flash 
{
	BOOL shouldFlash = (flash == false ? NO : YES);
	[[self.pipManager getPipWithPipID:pipID] requestPing:shouldFlash];
}

- (void) requestReset:(PipID) pipID
{
	[[self.pipManager getPipWithPipID:pipID] requestReset];
}

- (void) requestPowerOff:(PipID) pipID
{
	[[self.pipManager getPipWithPipID:pipID] requestPowerOff];
}

- (void) startStreaming:(PipID) pipID
{
	[[self.pipManager getPipWithPipID:pipID] startStreaming];
}

- (void) stopStreaming:(PipID) pipID
{
	[[self.pipManager getPipWithPipID:pipID] stopStreaming];
}

- (bool) isStreaming:(PipID) pipID
{
	BOOL streaming = [[self.pipManager getPipWithPipID:pipID] streaming];
    
    return (streaming == NO ? false : true);
}

#pragma mark PipDiscoveryProtocol

- (void) onPipManagerReady
{
    NSString *sendString  = [NSString stringWithFormat:@"pip_plugin:discovery:initialized:%d", PIP_RESULT_SUCCESS];
    UnitySendMessage("PipManager", "ReceiveDiscoveryMessage", [sendString UTF8String]);    
}

- (void) onPipDiscovered
{

    NSString *sendString  = [NSString stringWithFormat:@"pip_plugin:discovery:devicefound:%d", PIP_RESULT_SUCCESS];
    UnitySendMessage("PipManager", "ReceiveDiscoveryMessage", [sendString UTF8String]);    
}

- (void) onPipDiscoveryComplete
{
    NSString *sendString  = [NSString stringWithFormat:@"pip_plugin:discovery:complete:%d", PIP_RESULT_SUCCESS];
    UnitySendMessage("PipManager", "ReceiveDiscoveryMessage", [sendString UTF8String]);
}

- (void) onPipResumeComplete:(int) status
{
    NSString *sendString  = [NSString stringWithFormat:@"pip_plugin:discovery:resumed:%d", status];
    UnitySendMessage("PipManager", "ReceiveDiscoveryMessage", [sendString UTF8String]);
}

#pragma mark PipConnectionProtocol

- (void) onPipConnectedForPipID:(PipID) pipID withStatus:(NSInteger) status
{
    NSString *sendString  = [NSString stringWithFormat:@"pip_plugin:connection:connected:%d:%d", (int)status, (int)pipID];
    UnitySendMessage("PipManager", "ReceiveConnectionMessage", [sendString UTF8String]);
}

- (void) onPipDisconnectedForPipID:(PipID) pipID
{
    NSString *sendString  = [NSString stringWithFormat:@"pip_plugin:connection:disconnected:%d:%d", PIP_RESULT_SUCCESS, (int)pipID];
    UnitySendMessage("PipManager", "ReceiveConnectionMessage", [sendString UTF8String]);
}

#pragma mark PipControlProtocol
- (void) onPipBatteryEventForPipID:(PipID) pipID
{
    NSString *sendString  = [NSString stringWithFormat:@"pip_plugin:update:batteryevent:%d:%d", PIP_RESULT_SUCCESS, (int)pipID];
    UnitySendMessage("PipManager", "ReceiveUpdateMessage", [sendString UTF8String]);
}

- (void) onPipBatteryLevelForPipID:(PipID) pipID
{
    NSString *sendString  = [NSString stringWithFormat:@"pip_plugin:update:batterylevel:%d:%d", PIP_RESULT_SUCCESS, (int)pipID];
    UnitySendMessage("PipManager", "ReceiveUpdateMessage", [sendString UTF8String]);
}

- (void) onPipVersionForPipID:(PipID) pipID
{
    NSString *sendString  = [NSString stringWithFormat:@"pip_plugin:update:version:%d:%d", PIP_RESULT_SUCCESS, (int)pipID];
    UnitySendMessage("PipManager", "ReceiveUpdateMessage", [sendString UTF8String]);
}

- (void) onPipSetNameForPipID:(PipID) pipID
{
    NSString *sendString  = [NSString stringWithFormat:@"pip_plugin:update:setname:%d:%d", PIP_RESULT_SUCCESS, (int)pipID];
    UnitySendMessage("PipManager", "ReceiveUpdateMessage", [sendString UTF8String]);
    
}

- (void) onPipGetNameForPipID:(PipID) pipID
{
    NSString *sendString  = [NSString stringWithFormat:@"pip_plugin:update:getname:%d:%d", PIP_RESULT_SUCCESS, (int)pipID];
    UnitySendMessage("PipManager", "ReceiveUpdateMessage", [sendString UTF8String]);
}

- (void) onPipActivationChangedForPipID:(PipID) pipID
{
    NSString *sendString  = [NSString stringWithFormat:@"pip_plugin:update:activation:%d:%d", PIP_RESULT_SUCCESS, (int)pipID];
    UnitySendMessage("PipManager", "ReceiveUpdateMessage", [sendString UTF8String]);
}

- (void) onPipStreamingStatusChangedForPipID:(PipID) pipID
{
//    NSString *sendString  = [NSString stringWithFormat:@"pip_plugin:update:streamingChanged:%d", (int)pipID];
//    UnitySendMessage("PipManager", "ReceiveUpdateMessage", [sendString UTF8String]);
}

- (void) onPipPingForPipID:(PipID) pipID
{
    NSString *sendString  = [NSString stringWithFormat:@"pip_plugin:update:ping:%d:%d", PIP_RESULT_SUCCESS, (int)pipID];
    UnitySendMessage("PipManager", "ReceiveUpdateMessage", [sendString UTF8String]);
}

#pragma mark PipAnalyzerProtocol

- (void) onPipAnalyzerOutputEventForPipID:(PipID) pipID
{     
    int value = -1;
    
    Pip* pip = [self.pipManager getPipWithPipID:pipID];
    if ([pip active] == YES)
    {
	    NSMutableArray*outputs = [[self.pipManager getPipWithPipID:pipID] getAnalyzerOutput];
    	AnalyzerOutput* trend = [outputs objectAtIndex:0];
        value = (int)trend.value;
    }
     
    NSString *sendString  = [NSString stringWithFormat:@"pip_plugin:data:%d:%d:%d", PIP_RESULT_SUCCESS, (int)pipID, value];
    UnitySendMessage("PipManager", "ReceiveDataMessage", [sendString UTF8String]);
}

@end


