#!/bin/bash
psql -q -t -c 'SELECT pg_is_in_recovery();' | grep f

[[ $? == 1 ]] && ./promotePG1.sh || ./promotePG2.sh
