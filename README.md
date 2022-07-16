# [EcoServerEmulator](https://github.com/cm-MMK-2/EcoServerEmulator)  
<br />

# <span style="color: red; ">NOT a full server! ONLY for testing! </span>


## <span style="color: green; ">For the Memories of Emil Chronicle Online </span>
<br />

## Current Available: Login, Create Character, Move

  ![Pre](https://github.com/cm-MMK-2/EcoServerEmulator/blob/master/preview/progress.png)
<br />


## How to use
  1. Setup a mysql database and import /sql/eco.sql
  2. Setup connection string in /CommonLib/settings.json
  3. Build the project and launch WorldServer, LoginServer and MapServer.
<br />


## Client
  This emulator is compatible with the last version of japanese official eco client(2017/08/31), which can be downloaded [here](https://drive.google.com/file/d/18NU7MRoc79DAIjFVyVUb_q6cjzYEtdbz/view?usp=sharing).

### Steps for using the client
  1. Modify the address and port in server.lst file to make it the same as your server settings. 
  2. Start "eco.exe" with arguements `/launch -u:<username> -p:<password>`. (You can find an example in "start_eco.bat" batch file)
  3. Edit database and make sure the "account table" has your login account as added in step 2.


