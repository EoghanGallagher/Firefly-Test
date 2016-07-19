/*
 * PipEvents.h
 *
 * Copyright (c) 2014 Galvanic Ltd.
 * All rights reserved.
 *
 * This software is the confidential and proprietary information of Galvanic Limited.
 */

#import "PIPInfo.h"

/**
 * An object wishing to observe events raised in relation to PIP device discovery and
 * suspend/resume behaviour should inherit and implement the methods in this protocol.
 */
@protocol PipManagerProtocol <NSObject>

@required
/**
 * Callback invoked when the PIP management system is initialized and
 * ready to use.
 */
- (void) onPipManagerReady;

/**
 * Callback invoked when a PIP is identified during a discovery process.
 * The application can subsequently query the PipManager for details
 * of the discovered PIP.
 */
- (void) onPipDiscovered;

/**
 * Callback invoked when a PIP discovery process has completed.
 * An application can subsequently query the PipManager for
 * the details of discovered PIPs.
 */
- (void) onPipDiscoveryComplete;

/**
 * Callback invoked when PipManager has resumed from suspended state.
 *
 * @param status Resumption status: 0 indicates that the PIP system
 * has resumed successfully, -1 indicates that a failure has occurred.
 */
- (void) onPipResumeComplete:(int) status;

@end

/**
 * An object wishing to observe events raised in relation to PIP connection
 * lifetime management should inherit and implement the methods in this protocol.
 */
@protocol PipConnectionProtocol <NSObject>

@required

/**
 * Callback invoked when a connection attempt to a PIP has completed.
 *
 * @param pipID The unique identifier of the PIP raising the event.
 * @param status Integer code representing the status of the completed connection attempt. See
 * the PipConnectionStatus enumeration for possible values.
 */
- (void) onPipConnectedForPipID:(PipID) pipID withStatus:(NSInteger) status;

/**
 * Callback invoked when a PIP becomes disconnected.
 *
 * @param pipID The unique identifier of the PIP raising the event.
 */
- (void) onPipDisconnectedForPipID:(PipID) pipID;

@end

/**
 * An object wishing to observe events raised as a result of
 * PIP control events should inherit and implement the methods in this
 * protocol. Control events consist of either responses to asynchronous requests
 * made by the client or notifications originated by the PIP.
 */
@protocol PipControlProtocol <NSObject>


@required

/**
 * Callback invoked when a battery event notification has been received from PIP.
 * On receiving this event, an application can subsequently access the batteryEvent
 * property of the Pip class in order to identify the specific event raised.
 * @param pipID The unique identifier of the PIP that raised the event.
 */
- (void) onPipBatteryEventForPipID:(PipID) pipID;

/**
 * Callback invoked when a battery level request has completed.
 * On receiving this event, an application can subsequently access the batteryLevel
 * property of the Pip class in order to obtain the PIP's current battery level.
 * @param pipID The unique identifier of the PIP that raised the event.
 */
- (void) onPipBatteryLevelForPipID:(PipID) pipID;

/**
 * Callback invoked when a firmware version request has completed. 
 * On receiving this event, an application can subsequently access the version
 * property of the Pip class in order to obtain the PIP's firmware version.
 *
 * @param pipID The unique identifier of the PIP that raised the event.
 */
- (void) onPipVersionForPipID:(PipID) pipID;

/**
 * Callback invoked when a get name request has completed.
 * On receiving this callback, an application can get the name
 * by accessing the Pip class' info property.
 *
 * @param pipID The unique identifier of the PIP that raised the event.
 */
- (void) onPipGetNameForPipID:(PipID) pipID;

/**
 * Callback invoked when a set name request has completed. Note that
 * the PIP must be power-cycled in order for the change to take effect.
 *
 * @param pipID The unique identifier of the PIP that raised the event.
 */
- (void) onPipSetNameForPipID:(PipID) pipID;

/**
 * Callback invoked when a ping request has completed.
 *
 * @param pipID The unique identifier of the PIP that raised the event.
 */
- (void) onPipPingForPipID:(PipID) pipID;

/**
 * Callback invoked when the PIP's streaming status has changed.
 * An application can query the Pip class' streaming property in
 * order to determine whether or not the PIP device is currently
 * streaming.
 *
 * @param pipID The unique identifier of the PIP that raised the event.
 */
- (void) onPipStreamingStatusChangedForPipID:(PipID) pipID;

/**
 * Callback invoked when the PIP's activation status has changed.
 * An application can query the Pip class' active property in
 * order to determine whether or not the PIP device is currently
 * active.
 *
 * @param pipID The unique identifier of the PIP that raised the event.
 */
- (void) onPipActivationChangedForPipID:(PipID) pipID;

@end

/**
 * An object wishing to observe events raised by the PIP digital signal analysis
 * module should inherit and implement the methods in this protocol.
 */
@protocol PipAnalyzerProtocol <NSObject>

@required

/**
 * Callback function invoked when a PIP analyzer has updated one or more of
 * its output values.
 *
 * @param pipID The unique identifier of the PIP whose analyzer has generated the output.
 */
- (void) onPipAnalyzerOutputEventForPipID:(PipID) pipID;

@required

@end
