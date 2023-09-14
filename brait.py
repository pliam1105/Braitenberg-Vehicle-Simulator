import numpy as np
import cmath
import matplotlib.pyplot as plt
import random

class Function:
    def __init__(self, type, slope_up=-1, slope_down=-1, optx=-1, opty=-1):#type is "inc", "dec", or "inc_dec" or "dec_inc"
        self.type = type
        self.slope_up = slope_up
        self.slope_down = slope_down
        self.optx = optx
        self.opty = opty
    def evaluate(self, input):
        if(self.type == "inc"):
            return self.opty + self.slope_up*(input - self.optx)
        elif(self.type == "dec"):
            return self.opty + self.slope_down*(input - self.optx)
        elif(self.type == "inc_dec"):
            if(input<=self.optx):
                return self.opty + self.slope_up*(input - self.optx)
            else:
                return self.opty + self.slope_down*(input - self.optx)
        elif(self.type == "dec_inc"):
            if(input>self.optx):
                return self.opty + self.slope_up*(input - self.optx)
            else:
                return self.opty + self.slope_down*(input - self.optx)

class Sources:
    def __init__(self, numt, types, positions, consts):#numt is the maximum number of types (0...numt-1)
        self.numt = numt
        self.types = types
        self.positions = positions
        self.consts = consts
    def intensity(self, pos):#returns an array of the intensity of each type
        ret = np.zeros(self.numt)
        pos = np.asarray(pos)
        for i in range(len(self.positions)):
            type = self.types[i]
            const = self.consts[i]
            possource = np.asarray(self.positions[i])
            dist = np.linalg.norm(pos - possource)
            intens = const/(dist**2)
            ret[type] += intens
        return ret


class Sensor:
    def __init__(self, type, function):#type is an int that represents what the sensor measures, function is a Function object
        self.type = type
        self.function = function
    def output(self, pos, sources):
        return self.function.evaluate(sources.intensity(pos)[self.type])


class Vehicle:
    def __init__(self, sensors, connections, startlmpos, startrmpos, length):# sensors is a list of Sensor objects, connections is a list of 1 or -1, one for each sensor, 1 is "connect to same motor" and -1 "the opposite one"
        self.sensors = sensors
        self.connections = connections
        self.length = length
        #we start at (0,0), we consider the robot to be a rectangle where the sensors are in the top corners and the motors in the bottom corners
        self.left_motor_pos = np.asarray(startlmpos)
        self.right_motor_pos = np.asarray(startrmpos)
        self.width = np.linalg.norm(self.right_motor_pos-self.left_motor_pos)
    def sensor_pos(self, lmpos, rmpos):
        #calculate sensor positions using left and right motor positions
        clpos = complex(lmpos[0], lmpos[1])
        crpos = complex(rmpos[0], rmpos[1])
        clsens = (crpos-clpos)*1j*self.length/self.width+clpos
        crsens = clsens + crpos-clpos
        return np.asarray([clsens.real, clsens.imag]), np.asarray([crsens.real, crsens.imag])
    def velocities(self, sources, lmpos, rmpos):
        lsenspos, rsenspos = self.sensor_pos(lmpos, rmpos)
        lvel = rvel = 0
        for i in range(len(self.sensors)):
            sensor = self.sensors[i]
            conn = self.connections[i]
            lout = sensor.output(lsenspos, sources)
            rout = sensor.output(rsenspos, sources)
            if(conn == 1):
                lvel += lout
                rvel += rout
            else:
                lvel += rout
                rvel += lout
        return lvel, rvel
    def move_dt(self, sources, dt):
        lmpos, rmpos = self.left_motor_pos, self.right_motor_pos
        clmpos, crmpos = complex(lmpos[0], lmpos[1]), complex(rmpos[0], rmpos[1])
        lvel, rvel = self.velocities(sources, lmpos, rmpos)
        if(lvel != rvel):
            # we create a circle with center at the line connecting the motors such that the radial velocities at the motors are the intended velocities
            # we consider a line with points (0,0) and (1,0) for the sensors, and (l,0) for the center
            # then the center = l*(rmpos-lmpos)+lmpos
            # lvel/(0-l) = rvel/(1-l) -> lvel*(1-l) = rvel*(-l) -> lvel = l*(lvel-rvel) -> l = lvel/(lvel-rvel)
            # radv = lvel/(-l)
            l = lvel/(lvel-rvel)
            cmove = l*(crmpos-clmpos)+clmpos
            radv = lvel/(-l)
            dangle = radv*dt
            rotate_complex = cmath.exp(1j*dangle) # complex number to rotate the positions
            cnlmpos = cmove+(clmpos-cmove)*rotate_complex
            cnrmpos = cmove+(crmpos-cmove)*rotate_complex
        else:
            unitorth = 1j*(crmpos-clmpos)/self.width # unit complex orthogonal to the motor line
            cnlmpos = clmpos + unitorth*lvel
            cnrmpos = crmpos + unitorth*lvel
        nlmpos = np.asarray([cnlmpos.real, cnlmpos.imag])
        nrmpos = np.asarray([cnrmpos.real, cnrmpos.imag])
        self.left_motor_pos = nlmpos
        self.right_motor_pos = nrmpos
        return ((lmpos, rmpos), (nlmpos, nrmpos))

#test environment
sources = Sources(1, [0,0], [(-1,5), (-1,-5)], [1,1])
vehicles = [Vehicle([Sensor(0, Function("inc", 1, -1, 0, 0))], [-1], (0,0), (1,0), 1), Vehicle([Sensor(0, Function("dec", 1, -1, 0, 0))], [1], (0,0), (1,0), 1)]
for j, pos in enumerate(sources.positions):
    plt.scatter([pos[0]], [pos[1]], c="red")
    plt.annotate(str(j),(pos[0], pos[1]))
#generate random color map
cmap = {}
for j in range(len(vehicles)):
    r = random.random()
    g = random.random()
    b = random.random()
    cmap[j] = (r,g,b)
for i in range(100):
    for j, vehicle in enumerate(vehicles):
        ((_,_),(lmpos, rmpos)) = vehicle.move_dt(sources, 2)
        # print(lmpos, rmpos)
        pos = (lmpos+rmpos)/2
        plt.scatter([pos[0]], [pos[1]], c=[cmap[j]])
plt.show()
    