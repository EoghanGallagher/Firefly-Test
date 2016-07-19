//
//  BridgeWrapper.m
//
//  Created by Fiachra Matthews on 31/07/2013.
//  Copyright (c) 2016 Galvanic Ltd. All rights reserved.
//

#import "BridgeWrapper.h"
#import "PipController.h"


@implementation BridgeWrapper

extern "C" {
    
    const void iosBridge_initPlugin()
    {
        [PipController sharedManager];
    }
    
    const void iosBridge_discoverPips()
    {
		[[PipController sharedManager] discoverPips];
    }
	
    const void iosBridge_resetAll()
    {
        [[PipController sharedManager] resetAll];
    }
    
    const void iosBridge_cancelDiscovery()
    {
        [[PipController sharedManager] cancelDiscovery];
    }

    const void iosBridge_suspendPips()
    {
        [[PipController sharedManager] suspendPips];
    }

    const void iosBridge_resumePips()
    {
        [[PipController sharedManager] resumePips];
    }
	    
    const bool iosBridge_isDiscovering()
    {
		return [[PipController sharedManager] isDiscovering];
    }

    const bool iosBridge_isBluetoothActive()
    {
		return [[PipController sharedManager] isBluetoothActive];
    }
    
    const int iosBridge_getNumDiscoveredPips()
    {
        return [[PipController sharedManager] getNumDiscoveries];
    }
    
    const char* iosBridge_getNameForDiscoveredPip(int discoveredIndex)
    {
		return [[PipController sharedManager] getNameForDiscoveredPip:discoveredIndex];
    }
    
    const int iosBridge_getPipIDForDiscoveredPip(int discoveredIndex)
    {
		return [[PipController sharedManager] getPipIDForDiscoveredPip:discoveredIndex];
    }
    
    const int iosBridge_getNumRegisteredPips()
    {
		return [[PipController sharedManager] getNumRegistered];
    }
    
    const char * iosBridge_getNameForRegisteredPip(int registeredIndex)
    {
		return [[PipController sharedManager] getNameForRegisteredPip:registeredIndex];
    }
    
    const int iosBridge_getPipIDForRegisteredPip(int registeredIndex)
    {
		return [[PipController sharedManager] getPipIDForRegisteredPip:registeredIndex];
    }
    
    const void iosBridge_registerPip(int pipID)
    {
        [[PipController sharedManager] registerPip:pipID] ;
    }
    
    const void iosBridge_unregisterPip(int pipID)
    {
        [[PipController sharedManager] unregisterPip:pipID];
    }
    
    const void iosBridge_unregisterAll()
    {
        [[PipController sharedManager] unregisterAll] ;
    }
    	
	const char* iosBridge_getAllowedPipNameChars()
	{
		return [[PipController sharedManager] getAllowedPipNameChars];
	}
	
    const void iosBridge_connectPip(int pipID)
    {
        [[PipController sharedManager] connectPip:pipID];
    }
    
    const void iosBridge_disconnectPip(int pipID)
    {
        [[PipController sharedManager] disconnectPip:pipID];
    }
    
    const bool iosBridge_isPipConnected(int pipID)
    {
		return [[PipController sharedManager] isPipConnected:pipID];
    }
	
    const char * iosBridge_getName(int pipID)
    {
        return [[PipController sharedManager] getName:pipID];	
    }
    	
	const char* iosBridge_getVersion(int pipID)
	{
		return [[PipController sharedManager] getVersion:pipID];
	}
	
	const bool iosBridge_isPipActive(int pipID)
	{
		return [[PipController sharedManager] isPipActive:pipID];
	}
    
    const int iosBridge_getBatteryLevel(int pipID)
    {
		return [[PipController sharedManager] getBatteryLevel:pipID];
    }
	
    const int iosBridge_getBatteryEvent(int pipID)
    {
		return [[PipController sharedManager] getBatteryEvent:pipID];	
    }
    
    const void iosBridge_requestSetName(int pipID, const char* newName)
    {
		[[PipController sharedManager] requestSetName:pipID name:newName];
    }
    
    const void iosBridge_requestGetName(int pipID)
    {
        [[PipController sharedManager] requestGetName:pipID];
    }
	
    void iosBridge_requestVersion(int pipID)
    {
		return [[PipController sharedManager] requestVersion:pipID];
    }
    
    const void iosBridge_requestPowerOff(int pipID)
    {
		return [[PipController sharedManager] requestPowerOff:pipID]; 
    }
	
	const void iosBridge_requestReset(int pipID)
    {
		return [[PipController sharedManager] requestReset:pipID]; 
    }
		
    const void iosBridge_requestPing(int pipID, bool shouldflashLED)
    {
		return [[PipController sharedManager] requestPing:pipID flashLED:shouldflashLED]; 
    }

    const void iosBridge_startStreaming(int pipID)
    {
        [[PipController sharedManager] startStreaming:pipID];
    }
    
    const void iosBridge_stopStreaming(int pipID)
    {
		[[PipController sharedManager] stopStreaming:pipID]; 
	}

    const bool iosBridge_isStreaming(int pipID)
    {
		return [[PipController sharedManager] isStreaming:pipID];     
	}	
}

@end
