/*
 * PipManager.h
 *
 * Copyright (c) 2014 Galvanic Ltd.
 * All rights reserved.
 *
 * This software is the confidential and proprietary information of Galvanic Limited.
 */

#import <Foundation/Foundation.h>

#if TARGET_OS_IPHONE
#import <CoreBluetooth/CoreBluetooth.h>
#else
#import <IOBluetooth/IOBluetooth.h>
#endif

#import "Pip.h"

/**
 * This class enables an application to discover and register
 * PIP devices. An app can register a PipDiscoveryProtocol delegate
 * with PipManager in order to receive callbacks relating to
 * discovery operations. PipManager is implemented as a singleton.
 */
@interface PipManager : NSObject<CBCentralManagerDelegate>

/** 
 Get/set PipManager's PipManagerProtocol delegate. The discovery
 delegate allows an object to receive events relating to discovery of
 PIP devices.
 */
@property (nonatomic, weak) id<PipManagerProtocol> discoveryDelegate;

/**
 Property allowing an app to determine whether or not PIP discovery is in progress.
 */
@property (readonly) BOOL discovering;

/**
 The total number of discovered PIPs.
 */
@property (readonly) NSInteger numDiscoveries;

/**
 The total number of registered PIPs.
 */
@property NSInteger numRegistered;

/** 
 Property indicating whether or not PipManager is in the process
 of resuming PIP connection following an app suspend.
 */
@property (readonly) BOOL resuming;

/**
 Method allowing an app to determine whether or not the PipManager has been
 successfully initialized.
 */
+ (BOOL) initialized;


/**
 Initialize the PIPManager singleton. The first time an app requests a reference
 to the singleton, this method can be called to instantiate the singleton and also 
 supply a delegate implementing PipManagerProtocol as a parameter. Subsequent requests for
 the singleton can then be made using the sharedManager method.
 @param delegate A delegate implementing PipManagerProtocol.
 @return Referece to the PipManager singleton.
 */
+ (PipManager *) sharedManagerWithDelegate:(id<PipManagerProtocol>)delegate;

/**
 Get a reference to the PipManager singleton. If the singleton has not yet been instantiated,
 it will be created. If this method is used to instantiate the singleton, then the discoveryDelegate
 property should be used to set the PipDiscoveryProtocol delegate.
 @return Reference to PipManager singleton.
 */
+ (PipManager *) sharedManager;

/**
 Check if Bluetooth is active on the device.
 @return YES if Bluetooth is active, NO otherwise.
 */
- (BOOL) isBluetoothAdapterActive;

/** 
 Disconnect all currently connected PIPs and reset PipManager. This clears the
 list of previously discovered PIPs and re-loads the list of PIPs registered with the
 current app.
 */
- (void) disconnectAllAndReset;

/** 
 Kick off a Bluetooth discovery process to find PIP devices in range.
 */
- (void) discoverPips;



/**
 Cancel an in-progress discovery procedure.
 */
- (void) cancelDiscovery;

/**
 Get a PipInfo object for a discovered PIP.
 @param index The index in the list of discovered PIPs for which the PipInfo is required.
 Note that the index corresponds to the order in which PIPs were discovered. The first PIP 
 that was discovered will be at index zero, and so on.
 @return PipInfo for the registered PIP at the specified index.
 */
- (PipInfo *) getDiscoveryAtIndex:(NSInteger) index;

/**
 Get a PipInfo object corresponding to a PIP registered with the current app.
 @param index The index in the list of registered PIPs for which the PipInfo
 is required. Note that the index corresponds to the order in which PIPs were
 originally registered. The first PIP that was registered with the app
 will be at index zero, and so on.
 @return PipInfo for the registered PIP at the specified index.
 */
- (PipInfo *) getRegisteredPipAtIndex:(NSInteger) index;


/**
 Register a PIP with the current application for subsequent use.
 Typically, a user will always use the same PIP(s) with an app. By
 registering these PIPs, an application can reload their details 
 at a later time allowing the app to reconnect to them, without
 having to go through a discovery process.
 @param pipID The unique identifier of the PIP being registered.
 */
- (void) registerPipWithPipID:(PipID)pipID;

/**
 Remove a PIP from an application's list of registered PIPs.
 @param pipID The unique identifier of the PIP being un-registered.
 */
- (void) unregisterPipWithPipID:(PipID)pipID;

/**
 Remove all PIPs from an application's list of registered PIPs.
 */
- (void) unregisterAllPips;

/**
 Obtain a reference to an existing Pip object via its
 unique identifier.
 @param pipID The unique identifier of the PIP to retrieve.
 @return Pip A reference to the Pip object corresponding to the specified identifier, or
 nil if no such Pip object exists.
 */
- (Pip *) getPipWithPipID:(PipID)pipID;

/**
 Initiate a connection to a PIP with the specified unique identifier.
 @param pipID Unique identifier of the PIP to which to connect.
 */
- (void) connectPipWithPipID:(PipID)pipID;

/**
 Disconnect from a currently connected PIP.
 @param pipID Unique identifier of the PIP from which to disconnect.
 */
- (void) disconnectPipWithPipID:(PipID) pipID;

/**
 Prior to suspending, an application should call this method so that the PipManager
 can terminate any open connections to PIP devices and prepare them for restoration on
 resume.
 */
- (void) suspendPips;

/**
 On resuming from a suspended state, an application should call this method in order
 for the PipManager to automatically attempt to reconnect to any PIPs that were connected
 when the app was suspended.
 */
- (void) resumePips;

@end
