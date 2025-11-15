Serial3.setup(9600);
var buffer = '';

Serial3.on('data', function(data){
 //print(data); 
  if(buffer.indexOf('50005)')>-1) buffer='';
  
  buffer += data;
  if(data == ';'){
    print(buffer);
    try {eval(buffer);}
    catch(e){}
    buffer = '';
  }
});


PrimaryI2C.setup({sda: SDA, scl: SCL, bitrate: 400000});
var mServo = require('@amperka/multiservo').connect(PrimaryI2C);
var s0 = mServo.connect(0);// povorotniy stol
var s1 = mServo.connect(1);
var s2 = mServo.connect(2);
var s3 = mServo.connect(3);
var s4 = mServo.connect(4);// zahvat

var Motor = require('@amperka/motor');
var motorOne = Motor.connect(Motor.MotorShield.M1);
var motorTwo = Motor.connect(Motor.MotorShield.M2);

function go(w,s,a,d){
  var m1,m2 = 0;
  if(w){
    m1 = 1;
    m2 = 1;
  }
  else if(s){
    m1 = -1;
    m2 = -1;
  }
  else if(a){
    m1 = -1;
    m2 = 1;
  }
  else if(d){
    m1 = 1;
    m2 = -1;
  }

  motorOne.write(m1);
  motorTwo.write(m2);
}