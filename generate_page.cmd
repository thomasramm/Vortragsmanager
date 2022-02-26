@echo off
cd docs
bundle exec jekyll serve
cd ..

pause

rem Download RubyInstaller von rubyinstaller.org
rem nimm die Empfohlene Version Ruby+Devkit
rem Option 3 
rem Letzter Schritt im Installationswizard
rem > ridk install -> Enter (default)
rem 
rem in einer cmd folgenden Befehl eingeben
rem > gem install jekyll bundler
rem > bundle install
rem danach testen mit
rem > jekyll -v

rem Download git von https://git-scm.com/download/win