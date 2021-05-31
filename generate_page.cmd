@echo off
cd docs
bundle exec jekyll serve
cd ..

pause

rem Download RubyInstaller von rubyinstaller.org
rem nimm die Empfohlene Version Ruby+Devkit
rem Letzter Schritt im Installationswizard
rem > ridk install -> Enter (default)
rem 
rem in einer cmd folgenden Befehl eingeben
rem > gem install jekyll bundler
rem danach testen mit
rem > jekyll -v