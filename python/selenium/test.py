from selenium import webdriver
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.common.exceptions import TimeoutException
import time


class Test:
    chrome = webdriver

    def __init__(self, nav):
        self.chrome = nav

    def title(self, name):
        assert name in self.chrome.title
        print("\t"+name+" expected: "+self.chrome.title)

    def connection(self):
        self.title("Bibliotheque")

    def donneesession(self, data):
        for i in data:
            print("\t"+i+":\t"+data[i]+" expected: "+self.chrome.find_element_by_name(i).get_attribute('value'))
            assert data[i] in self.chrome.find_element_by_name(i).get_attribute('value')
