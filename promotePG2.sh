#!/bin/bash

pg_ctlcluster 9.5 main stop;
ssh postgres@pg2 'pg_ctlcluster 9.5 main promote';
sleep 2;
mv ~/9.5/main/recovery.done ~/9.5/main/recovery.conf
pg_ctlcluster 9.5 main start;
