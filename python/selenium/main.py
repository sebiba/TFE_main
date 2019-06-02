# from multiprocessing import Process
import multiprocessing
from selenium import webdriver
from selenium.common.exceptions import NoSuchElementException
from action import Action
from test import Test
import time


def setBrowser():
    browser = webdriver.Chrome("D:\programmation\chromedriver")
    browser.get('https://localhost:44371')
    return browser


def main(pseudo, mdp):
    browser = setBrowser()
    test = Test(browser)
    test.title('Home Page')
    action = Action(browser)
    action.connection(pseudo, mdp)
    test.connection()
    browser.quit()


def wrongPassword(pseudo, mdp):
    browser = setBrowser()
    test = Test(browser)
    test.title('Home Page')
    action = Action(browser)
    action.connection(pseudo, mdp)
    assert "Bibliotheque" not in browser.title
    browser.quit()


if __name__ == "__main__":
    multiprocessing.Process(target=main, args=('sebiba@gmail.com', 'Sebiba1330#')).start()
    multiprocessing.Process(target=wrongPassword, args=('wrong', 'wrong123#')).start()
