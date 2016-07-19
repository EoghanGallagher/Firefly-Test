/*
 * Pip.h
 *
 * Copyright (c) 2014 Galvanic Ltd.
 * All rights reserved.
 *
 * This software is the confidential and proprietary information of Galvanic Limited.
 */

#import <Foundation/Foundation.h>

#import "PIPEvents.h"

#if TARGET_OS_IPHONE
#import <CoreBluetooth/CoreBluetooth.h>
#else
#import <IOBluetooth/IOBluetooth.h>
#endif

/** 
 Enumeration specifying the list of possible battery events that can be raised by the
 PIP device.
 */
typedef NS_ENUM(NSUInteger,PipBatteryEvent)
{
    /// The PIP's battery level is low.
	PIP_BE_BATTERY_LOW = 0,
    /// A charging device has been attached to the PIP.
	PIP_BE_CHARGER_ATTACH = 1,
    /// A charging device has been detached from the PIP.
	PIP_BE_CHARGER_DETACH = 2,
    /// A previously crictical or low battery level has been restored to a normal level.
	PIP_BE_BATTERY_OK = 3,
    /// The battery level is critically low.
    PIP_BE_BATTERY_CRITICAL = 4,
    /// No battery event has been received from the PIP.
    PIP_BE_NONE = 5,
};

/**
 Enumeration specifying the list of connection statuses after an attempted connection
 */
typedef NS_ENUM(int, PipConnectionStatus)
{
    /// The connection attempt completed successfully.
    PIP_CS_OK = 0,
    /// The connection attempt timed out.
    PIP_CS_TIMEOUT,
    /// The connection attempt failed due to a pairing error.
    PIP_CS_PAIRING_FAILED,
    /// The connection attempt failed for an unspecified reason.
    PIP_CS_UNKNOWN
};

/**
 * This class encapsulates bi-directional communication with a PIP device,
 * including sending of control requests, handling of responses and notifications
 * and streaming of sample data. Applications do not instantiate Pip objects directly.
 * Instead, the getPipWithPipID method of the PipManager class should be used.
 */
@interface Pip : NSObject

/** 
 Determine whether the PIP is currently streaming data samples. The value of this property
 will be YES if the device is currently streaming, NO otherwise.
 */
@property BOOL streaming;

/** 
 Determine whether the PIP is currently connected. The value of this property will be YES if the
 device is currently streaming, NO otherwise.
 */
@property BOOL connected;

/**
 Determine if the connection to the PIP is currently in a suspended state. The value of this 
 property will be YES if the connection is in the suspended state, NO otherwise.
 */
@property (readonly) BOOL connectionSuspended;

/**
 Determine if the PIP was streaming data before it entered the suspended state. The value of this
 property will be YES if the device was streaming, NO otherwise.
 */
@property (readonly) BOOL streamingBeforeSuspend;;

/**
 Retrieve the PIP's battery charge level. This will be a percentage value between 0 and 100,
 where 100 represents fully charged. The SDK automatically request the battery level at regular 
 intervals and invokes the onPipBatteryLevelForPipID method of PipControlProtocol when a new
 reading is available.
 */
@property (readonly) NSInteger batteryLevel;

/** 
 Retrieve the most recent battery event raised by the PIP. 
 */
@property (readonly) PipBatteryEvent batteryEvent;

/**
 Retrieve a string representation of the firmware version running on
 a connected PIP. The version string is in the format MAJOR.MINOR.SUBMINOR
 where each component of the string consists of at least one digit and at most
 two digits.
 */
@property (readonly, nonatomic, strong) NSString *version;

/**
 Retrieve the PipInfo object for this PIP.
 */
@property (nonatomic, strong) PipInfo *info;

/**
 Retrieve the most recent analyzer output. This is an array of AnalyzerOutput objects,
 each of which consists of a floating point output value and a string label identifying
 what the output represents.
 */
@property(nonatomic, strong, readonly, getter = getAnalyzerOutput) NSMutableArray* analyzerOutput;

/** 
 This property allows a delegate which implements the PipConnectionProtocol to
 register itself with a Pip object, in order to receive connection related events.
 */
@property (nonatomic, weak) id<PipConnectionProtocol> connectionDelegate;

/** 
 This property allows a delegate which implements the PipControlProtocol to
 register itself with a Pip object, in order to receive events related to
 PIP requests and notifications.
 */
@property (nonatomic, weak) id<PipControlProtocol> controlDelegate;

/** This property allows a delegate which implements the PipAnalyzerProtcol to
 register itself with a Pip object, in order to receive an event when a signal
 analyzer has new output available. 
 */
@property (nonatomic, weak) id<PipAnalyzerProtocol> analyzerDelegate;

/**
 Determine whether or not the PIP device's sensor discs are being held.
 */
@property (readonly) BOOL active;

/**
 Retrieve the set of characters that are permitted in the PIP name.
 @return NSCharacterSet comprising all permitted PIP name characters.
 */
+ (NSCharacterSet *) allowedNameChars;

/**
 Retrieve string representation of the set of characters that are permitted in the PIP name.
 @return NSString comprising all permitted PIP name characters.
 */

+ (NSString *) allowedNameCharsAsString;

/**
 Get the discovery index of this Pip object in the list of discovered devices.
 @return Integer index of this Pip object in the discovery list.
 */
- (NSInteger) getDiscoveryIndex;

/**
 Get the registered index of this Pip object in the list of devices registered to the current
 application.
 @return Integer index of this Pip object in the registered PIP list.
 */
-(NSInteger) getRegisteredIndex;

/**
 Issue an asynchronous request to a connected PIP to commence data sample streaming.
 */
- (void) startStreaming;

/**
 Issue an asynchronous request to a connected PIP to cease data sample streaming.
 */
- (void) stopStreaming;

/**
 Issue an ansynchronous request to a connected PIP for its Bluetooth "friendly" name.
 */
- (void) requestName;

/**
 Issue an ansynchronous request to a connected PIP for its firmware version.
 */
- (void) requestVersion;

/**
 Issue an ansynchronous request to a connected PIP to change its Bluetooth "friendly" name.
 When such a request is successful, the name change does not take effect until the PIP is
 powered cycled. An application's UI should inform the user accordingly.
 @param name The new name for the PIP.
 @return BOOL YES if the requested name is valid and the request succeeded.
 */
- (BOOL) requestSetName:(NSString*)name;

/**
 Issue an ansynchronous ping request to a connected PIP. This can take the form of a customary
 "heartbeat" transaction or, optionally, the PIP can flash its status LED to indicate receipt
 of the request.
 @param flashLED Set to YES if the PIP should flash its status LED, set to NO for a "silent" ping.
 */
- (void) requestPing:(BOOL) flashLED;

/**
 Issue an ansynchronous request to a connected PIP to reset itself. A reset will clear the PIP's
 paired device list and power down the PIP.
 */
- (void) requestReset;

/**
 Issue an ansynchronous request to a connected PIP to power itself down.
 */
- (void) requestPowerOff;



@end
