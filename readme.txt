можно было бы ещё прилизать но время уже нет. 
долго разбирался с запуском и настройкой коммуникации с bitcoind. 
до сих пор не всё ясно с ним.

бд создавал так:
db table script: db.txt
db user: test/test
dbname:btcsrvdb
ConnectionStrings: BtcWebSrvApp/web.config, UnitTestProject/WebAppConnectionStringProvider


сервера запускал так:
bitcoind.exe -rest -rpcuser=btcusr -rpcpassword=btcpwd -regtest -rpcport=11111 -rpcallowip=192.168.0.45 -connect=192.168.0.45 -server
bitcoind.exe -rest -rpcuser=btcusr -rpcpassword=btcpwd -regtest -rpcport=11112 -datadir=./data2 -rpcallowip=192.168.0.45 -server


создавал начальные данные по кошелькам, адресам и герерил начальные балансы на них методом теста:
UnitTest1.cs/InitWalletsInfo_Call_Success
вначале метода конфигурация кошельков


проверял так:
$.ajax({url:'/api/btcsrv/sendbtc',type:'POST',data:{toaddress:'2Muj1e6eoh9pbNdW2wScJnQFaBnqhgEdXTk',amount:0.0000055},success:function(resp){console.log(resp);}});

$.ajax({url:'/api/btcsrv/getlast',type:'GET',data:{},success:function(resp){console.log(resp);}});

