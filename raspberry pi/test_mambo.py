from pyparrot.Minidrone import Mambo

# If you are using BLE: you will need to change this to the address of YOUR mambo
# if you are using Wifi, this can be ignored

#mamboAddr = "8c:85:90:79:78:49" #wifi
mamboAddr = "D0:3A:08:D3:E6:26" #bluetooth

# make my mambo object
# remember to set True/False for the wifi depending on if you are using the wifi or the BLE to connect
mambo = Mambo(mamboAddr, use_wifi=False)

success = mambo.connect(num_retries=3)
print("connected: %s" % success)

if (success):
    
    mambo.smart_sleep(5)
    print("Flying: taking off!")
    mambo.safe_takeoff(4)
    mambo.ask_for_state_update()
    mambo.smart_sleep(4)
    
    print("Flying: going up")
    mambo.fly_direct(roll=0, pitch=0, yaw=0, vertical_movement=30, duration=1)
    mambo.smart_sleep(4)
    
    print("Flying: going down")
    mambo.fly_direct(roll=0, pitch=0, yaw=0, vertical_movement=-30, duration=1)
    mambo.smart_sleep(4)
    
    print("Flying: positive roll")
    mambo.fly_direct(roll=20, pitch=0, yaw=0, vertical_movement=0, duration=1)
    mambo.smart_sleep(4)
    
    print("Flying: negative roll")
    mambo.fly_direct(roll=-20, pitch=0, yaw=0, vertical_movement=0, duration=1)
    mambo.smart_sleep(4)
    
    print("Flying: positive pitch")
    mambo.fly_direct(roll=0, pitch=20, yaw=0, vertical_movement=0, duration=1)
    mambo.smart_sleep(4)
    
    print("Flying: negative pitch")
    mambo.fly_direct(roll=0, pitch=-20, yaw=0, vertical_movement=0, duration=1)
    mambo.smart_sleep(4)
    
    print("Flying: clockwise yaw")
    mambo.fly_direct(roll=0, pitch=0, yaw=180, vertical_movement=0, duration=1)
    mambo.smart_sleep(4)
    
    print("Flying: anti-clockwise yaw")
    mambo.fly_direct(roll=0, pitch=0, yaw=-180, vertical_movement=0, duration=1)
    mambo.smart_sleep(4)
    
#    print("Flying: right flip")
#    mambo.flip(direction="right")
#    mambo.smart_sleep(4)
    
#    print("Flying: left flip")
#    mambo.flip(direction="left")
#    mambo.smart_sleep(4)
    
#    print("Flying: front flip")
#    mambo.flip(direction="front")
#    mambo.smart_sleep(4)
    
#    print("Flying: back flip")
#    mambo.flip(direction="back")
#    mambo.smart_sleep(4)
    
    print("landing")
    mambo.safe_land(4)
    mambo.smart_sleep(4)
    
    print("disconnect")
    mambo.disconnect()

