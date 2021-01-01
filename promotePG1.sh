#!/bin/bash

ssh postgres@pg2 'pg_ctlcluster 9.5 main stop;';
pg_ctlcluster 9.5 main promote;
sleep 2;
ssh postgres@pg2 'mv ~/9.5/main/recovery.done ~/9.5/main/recovery.conf'
ssh postgres@pg2 'pg_ctlcluster 9.5 main start;';
