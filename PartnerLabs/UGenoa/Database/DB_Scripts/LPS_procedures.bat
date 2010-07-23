echo LPS Procedures: %1

isqlw -E -d %1 -i .\ProcessAgent\ProcessAgentProcedures.sql -o LPSBuild2.log

