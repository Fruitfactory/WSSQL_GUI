@echo off

SETLOCAL

set es_java_home=%~5

echo %es_java_home%

set SCRIPT_DIR=%~dp0
for %%I in ("%SCRIPT_DIR%..") do set ES_HOME=%%~dpfI

TITLE Elasticsearch Plugin Manager 1.3.6

"%es_java_home%\bin\java" %JAVA_OPTS% -Xmx64m -Xms16m -Des.path.home="%ES_HOME%" -cp "%ES_HOME%/lib/*;" "org.elasticsearch.plugins.PluginManager" %1 %2 %3 %4
goto finally


:err
echo JAVA_HOME environment variable must be set!

:finally

ENDLOCAL