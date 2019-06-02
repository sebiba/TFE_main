from selenium.webdriver.common.keys import Keys
from selenium import webdriver


class Action:
    browser = webdriver

    def __init__(self, browser):
        self.browser = browser

    def connection(self, pseudo, mdp):
        self.browser.find_element_by_id("loginLink").click()
        self.browser.find_element_by_id("Email").send_keys(pseudo)  # + Keys.RETURN non car soumission du form
        self.browser.find_element_by_id("Password").send_keys(mdp + Keys.RETURN)

    def inscription(self, nom, prenom, date, email, pseudo, mdp, mdp2):
        self.browser.find_element_by_name("nom").send_keys(nom)
        self.browser.find_element_by_name("prenom").send_keys(prenom)
        self.browser.find_element_by_name("date").send_keys(date)
        self.browser.find_element_by_name("email").send_keys(email)
        self.browser.find_element_by_name("pseudo").send_keys(pseudo)
        self.browser.find_element_by_name("mdp").send_keys(mdp)
        self.browser.find_element_by_name("mdp2").send_keys(mdp2)
        self.browser.find_element_by_name("inscription_client").click()

    def recherchepartenaire(self, data):
        for i in data:
            self.browser.find_element_by_id(i).clear()
            self.browser.find_element_by_id(i).send_keys(data[i])
        self.button('submit')

    def changedonnee(self, data):
        for i in data:
            self.browser.find_element_by_name(i).clear()
            self.browser.find_element_by_name(i).send_keys(data[i])
        self.button('change')

    def goto(self, id, metode='id'):
        if metode == 'id':
            self.browser.find_element_by_id(id).click()
        else:
            self.browser.find_element_by_xpath("//input[@type='button' and @value='"+id+"']")

    def button(self, name):
        self.browser.find_element_by_name(name).click()
