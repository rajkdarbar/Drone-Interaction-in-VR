from pyparrot.Minidrone import Mambo
import socket
import time

# If you are using BLE: you will need to change this to the address of YOUR mambo
# if you are using Wifi, this can be ignored

#mamboAddr = "8c:85:90:79:78:49" #wifi
mamboAddr = "D0:3A:08:D3:E6:26" #bluetooth

# make my mambo object
# remember to set True/False for the wifi depending on if you are using the wifi or the BLE to connect
mambo = Mambo(mamboAddr, use_wifi=False)

# UDP server global variable
UDP_IP = '0.0.0.0'
UDP_PORT = 5035
BUFFER_SIZE = 1024
FRAME_RATE = 0.03
MESSAGE = "Hello! Socket. Hope you are doing great!"

DEBUG = False

# UDP socket creation 
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM) # Internet, UDP
sock.settimeout(0.001)
sock.bind((UDP_IP, UDP_PORT))

# connection to the drone
success = True
if(not DEBUG):
    success = mambo.connect(num_retries=3)
print("connected: %s" % success)

if (success):
    # takeoff
    if(not DEBUG):
        mambo.safe_takeoff(2)
        mambo.smart_sleep(2)
    loopTime = time.clock()

    # running loop
    while True:
        try:
            # try receiving data
            data, addr = sock.recvfrom(BUFFER_SIZE)
            if(time.clock() - loopTime >= FRAME_RATE):
                loopTime = time.clock()
                goal = data.decode('utf-8').split(" ")

                # data is valid
                if len(goal) == 4 :
                    print("x:"+str(int(goal[0]))+" y:"+str(int(goal[1]))+" z:"+str(int(goal[2]))+" o:"+str(int(goal[3])))
                    if(not DEBUG):
                        #mambo.fly_direct(roll=int(goal[2]), pitch=-int(goal[0]), yaw=int(goal[3]), vertical_movement=int(goal[1]), duration=FRAME_RATE)
                        mambo.fly_direct(roll=int(goal[0]), pitch=int(goal[2]), yaw=int(goal[3]), vertical_movement=int(goal[1]), duration=FRAME_RATE)
                else:
                    print("invalid data: " + str(data))
            
        # quit order or timeout (no message incoming)
        except KeyboardInterrupt:
            break
        except:
            pass

    #landing and quit
    sock.close()
    if(not DEBUG):
        mambo.safe_land(4)
        mambo.smart_sleep(4)
        mambo.disconnect()
    
