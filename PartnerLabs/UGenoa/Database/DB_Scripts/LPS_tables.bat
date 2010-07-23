echo populating LPS %1

isqlw -E -d %1 -i .\ProcessAgent\ProcessAgentTables.sql -o LPSBuild0.log
isqlw -E -d %1 -i .\ProcessAgent\SetdefaultsPA.sql -o LPSBuild1.log
