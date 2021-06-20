import networkx as nx
from networkx.algorithms import tournament
import matplotlib.pyplot as plt
import random
from copy import deepcopy
from threading import Thread


def createGraph():
    n = random.randint(3, 10)
    p = 0.3
    G = nx.DiGraph()
    G.add_nodes_from([i for i in range(n)])
    for i in G.nodes():
        for j in G.nodes():
            if i != j:
                if random.random() < p:
                    G.add_edge(i, j)
    return G


myG = createGraph()
x = tournament.random_tournament(10)
nx.draw_networkx(x)
plt.savefig("filename.png")

def recursiveFunction(graph, path):
    threads = []
    results = []
    if len(path) == len(graph.nodes):
        return
    for adjacentValue in graph.adj[path[-1]]:
        if adjacentValue not in path:
            xP = deepcopy(path)
            xP.append(adjacentValue)
            results.append(xP)
            threads.append(Thread(target=recursiveFunction, args=(graph, xP)))
            threads[-1].start()

    for i in range(len(threads)):
        threads[i].join()
        res = results[i]
        if len(res) == len(graph.nodes):
            path.clear()
            path.extend(res)
            return


def hamiltonCycle(graph):
    path = [0]
    recursiveFunction(graph, path)
    if len(path) != len(graph.nodes):
        print("Solution does not exist\n")
        return False
    printSolution(path)
    return True


def printSolution(path):
    print("Solution Exists: Following",
          "is one Hamiltonian Cycle")
    for vertex in path:
        print(vertex, end=" ")
    print(path[0], "\n")


hamiltonCycle(myG)
